using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Future
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] List<Sprite> m_Sprites = new List<Sprite>();
        [SerializeField] Tile m_TilePrefab;

        [SerializeField] int m_GridSizeX = 8;
        [SerializeField] int m_GridSizeY = 8;

        [SerializeField] float m_ScaleMultiplier = 5f;
        [SerializeField] float m_Distance = 1.0f;

        private Tile[,] m_Grid;

        private void Start()
        {
            InitGrid();
        }

        void InitGrid()
        {
            m_Grid = new Tile[m_GridSizeX, m_GridSizeY];

            Vector3 positionOffset = transform.position - new Vector3(m_GridSizeX * m_Distance / 2.0f, m_GridSizeY * m_Distance / 2.0f, 0); // 1
            for (int row = 0; row < m_GridSizeX; row++)
            {
                for (int column = 0; column < m_GridSizeY; column++)
                {
                    Tile newTile = Instantiate(m_TilePrefab);

                    int index = Random.Range(0, m_Sprites.Count);
                    newTile.transform.parent = transform;
                    newTile.transform.position = new Vector3(row * m_Distance, column * m_Distance, 0) + positionOffset;
                    newTile.transform.localScale = Vector3.one * m_ScaleMultiplier;

                    newTile.Init(m_Sprites[index], index, row, column, DestroyAllNeighbours);

                    m_Grid[row, column] = newTile;
                }
            }
        }

        HashSet<Tile> m_VisitedTiles;
        List<Tile> m_ValidNeighbours;

        void DestroyAllNeighbours(int coordX, int coordY, int id)
        {
            Tile tappedTile = m_Grid[coordX, coordY];

            m_VisitedTiles = new HashSet<Tile>();
            m_ValidNeighbours = new List<Tile>();

            m_CurrentRecursion = 0;

            DFS(tappedTile);

            foreach (var item in m_ValidNeighbours)
            {
                item.SetInvisible();
            }

            RecalculateGrid();
        }

        void RecalculateGrid()
        {
            HashSet<int> visitedColumns = new HashSet<int>();

            m_ValidNeighbours = m_ValidNeighbours.OrderBy(x => (int)x.GetCoordinates().x).ToList();
            m_ValidNeighbours = m_ValidNeighbours.OrderBy(x => (int)x.GetCoordinates().y).ToList();

            Dictionary<int, int> emptySpacesPerColumn = new Dictionary<int, int>();

            foreach (var item in m_ValidNeighbours)
            {
                var coodinates = item.GetCoordinates();
                int coordX = (int)coodinates.x;
                int coordY = (int)coodinates.y;

                if (visitedColumns.Contains(coordX))
                    continue;

                visitedColumns.Add(coordX);

                int emptySpaces = 1;

                while (coordY < m_Grid.GetLength(1) - 1)
                {
                    coordY++;
                    Tile temp = m_Grid[coordX, coordY];
                    if (!temp.IsVisible())
                    {
                        emptySpaces++;
                    }
                }

                emptySpacesPerColumn.Add(coordX, emptySpaces);
                Debug.Log(string.Format("Found {0} empty spaces in column {1}!", emptySpaces, coordX));
            }

            foreach (var item in emptySpacesPerColumn)
            {
                int i = 0;
                int emptySpaces = 0;

                int itemValue = item.Value; // emptySpacesFound

                int recusionGuard1 = 0;
                int maxRecursions1 = 1000;
                while (itemValue > 0)
                {
                    recusionGuard1++;
                    if (recusionGuard1 > maxRecursions1)
                        return;

                    if (!m_Grid[item.Key, i].IsVisible())
                    {
                        emptySpaces++;
                    }
                    else
                    {
                        Tile temp;
                        temp = m_Grid[item.Key, i];

                        int j = i;

                        int recusionGuard = 0;
                        int maxRecursions = 1000;

                        while (temp.IsVisible() && j < m_Grid.GetLength(1))
                        {
                            recusionGuard++;
                            if (recusionGuard > maxRecursions)
                                return;
                            temp.PlayMoveDownAnimation(emptySpaces);
                            j++;
                            try
                            {
                                temp = m_Grid[item.Key, j];
                            }
                            catch
                            {
                                Debug.Log("WTF");
                            }
                        }

                        itemValue -= emptySpaces;
                    }

                    i++;
                }
            }
        }

        int m_CurrentRecursion = 0;
        int m_DebugRecursionLimit = 100000;

        void DFS(Tile currentTile)
        {
            m_CurrentRecursion++;

            if (m_CurrentRecursion > m_DebugRecursionLimit)
            {
                Debug.LogError("Infinite recursion!");
                return;
            }

            m_VisitedTiles.Add(currentTile);

            m_ValidNeighbours.Add(currentTile);

            var coord = currentTile.GetCoordinates();
            var nieghbours = GetAllNeighbours((int)coord.x, (int)coord.y);

            for (int i = 0; i < nieghbours.Count; i++)
            {
                if (m_VisitedTiles.Contains(nieghbours[i]))
                    continue;

                if (nieghbours[i].GetID() == currentTile.GetID())
                    DFS(nieghbours[i]);
            }
        }

        List<Tile> GetAllNeighbours(int x, int y)
        {
            List<Tile> neighbours = new List<Tile>();
            if (x > 0)
                neighbours.Add(m_Grid[x - 1, y]);
            if (x < m_Grid.GetLength(0) - 1)
                neighbours.Add(m_Grid[x + 1, y]);
            if (y > 0)
                neighbours.Add(m_Grid[x, y - 1]);
            if (y < m_Grid.GetLength(1) - 1)
                neighbours.Add(m_Grid[x, y + 1]);

            return neighbours;
        }
    }
}