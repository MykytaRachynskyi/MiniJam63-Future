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

        [SerializeField] int m_RequiredNeighboursToDestroy = 3;

        [SerializeField] int m_FallDistance = 20;

        private Tile[,] m_Grid;

       /* private void Start()
        {
            InitGrid();
        }*/

        public void ReinitGrid()
        {
            DestroyGrid();
            InitGrid();
        }

        void DestroyGrid()
        {
            if (m_Grid == null || m_Grid.Length == 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < m_Grid.GetLength(0); i++)
                {
                    for (int j = 0; j < m_Grid.GetLength(1); j++)
                    {
                        Destroy(m_Grid[i, j].gameObject);
                    }
                }
            }
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
                    newTile.transform.position = new Vector3(row * m_Distance, column * m_Distance + m_FallDistance, 0) + positionOffset;
                    newTile.InitPlayMoveDownAnimation(m_GridSizeY, m_Distance, InitOnTileFinishedFalling);
                    newTile.transform.localScale = Vector3.one * m_ScaleMultiplier;

                    newTile.Init(m_Sprites[index], index, row, column, DestroyAllNeighbours);
                    m_Grid[row, column] = newTile;
                }
            }
        }

        HashSet<Tile> m_VisitedTiles;
        List<Tile> m_ValidNeighbours;

        Dictionary<int, int> m_EmptySpacesPerColumn = new Dictionary<int, int>();
        HashSet<Tile> m_TilesBeingAnimated = new HashSet<Tile>();
        HashSet<Tile> m_InitTilesBeingAnimated = new HashSet<Tile>();

        void DestroyAllNeighbours(int coordX, int coordY, int id)
        {
            Tile tappedTile = m_Grid[coordX, coordY];

            m_VisitedTiles = new HashSet<Tile>();
            m_ValidNeighbours = new List<Tile>();

            m_TilesBeingAnimated.Clear();

            m_CurrentRecursion = 0;

            DFS(tappedTile);

            if (m_ValidNeighbours.Count < m_RequiredNeighboursToDestroy)
            {
                Debug.Log("Not enough neighbours");
                return;
            }

            SetAllTilesInteractable(false);

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

            m_EmptySpacesPerColumn.Clear();

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

                m_EmptySpacesPerColumn.Add(coordX, emptySpaces);
                Debug.Log(string.Format("Found {0} empty spaces in column {1}!", emptySpaces, coordX));
            }

            foreach (var item in m_EmptySpacesPerColumn)
            {
                int i = 0;
                int processedEmptySpaces = 0;

                int unprocessedEmptySpaces = item.Value; // emptySpacesFound

                int recusionGuard1 = 0;
                int maxRecursions1 = 1000;

                while (unprocessedEmptySpaces > 0)
                {
                    // Recursion guard
                    {
                        recusionGuard1++;
                        if (recusionGuard1 > maxRecursions1)
                        {
                            Debug.LogError("Inifinite recursion!");
                            return;
                        }
                    }
                    // ================

                    bool invisible = false;
                    try
                    {
                        invisible = !m_Grid[item.Key, i].IsVisible();
                    }
                    catch
                    {
                        break;
                    }

                    if (invisible)
                    {
                        processedEmptySpaces++;
                        i++;
                    }
                    else
                    {
                        Tile temp;
                        temp = m_Grid[item.Key, i];

                        int j = i;

                        int recusionGuard = 0;
                        int maxRecursions = 1000;

                        int processedTiles = 0;

                        while (temp.IsVisible() && j < m_Grid.GetLength(1))
                        {
                            // Recursion guard
                            {
                                recusionGuard++;
                                if (recusionGuard > maxRecursions)
                                {
                                    Debug.LogError("Inifinite recursion!");
                                    return;
                                }
                            }
                            // ================

                            try
                            {
                                if (processedEmptySpaces > 0)
                                {
                                    m_TilesBeingAnimated.Add(temp);
                                    temp.PlayMoveDownAnimation(processedEmptySpaces, m_Distance,
                                        m_Grid[item.Key, j - processedEmptySpaces], OnTileFinishedFalling);
                                }
                                j++;
                                temp = m_Grid[item.Key, j];
                                processedTiles++;
                            }
                            catch
                            {
                                Debug.Log("WTF");
                            }
                        }

                        i += processedTiles;
                        unprocessedEmptySpaces -= processedEmptySpaces;
                    }
                }
            }

            if (m_TilesBeingAnimated.Count == 0)
            {
                m_TilesBeingAnimated.Clear();
                RepopulateGrid();
            }
        }

        void OnTileFinishedFalling(Tile tile)
        {
            if (m_TilesBeingAnimated.Contains(tile))
            {
                m_TilesBeingAnimated.Remove(tile);
            }

            Debug.Log(m_TilesBeingAnimated.Count);

            if (m_TilesBeingAnimated.Count == 0)
            {
                RepopulateGrid();
            }
        }

        void InitOnTileFinishedFalling(Tile tile)
        {
            if (m_InitTilesBeingAnimated.Contains(tile))
            {
                m_InitTilesBeingAnimated.Remove(tile);
            }

            Debug.Log(m_InitTilesBeingAnimated.Count);

            if (m_InitTilesBeingAnimated.Count == 0)
            {
                SetAllTilesInteractable(true);
            }
        }

        void RepopulateGrid()
        {
            foreach (var item in m_EmptySpacesPerColumn)
            {
                int j = 1;
                for (int i = 0; i < item.Value; i++)
                {
                    int index = Random.Range(0, m_Sprites.Count);
                    int row = m_Grid.GetLength(1) - j;
                    m_Grid[item.Key, row].transform.position += new Vector3(0, m_FallDistance + 2, 0);
                    m_Grid[item.Key, row].Init(m_Sprites[index], index, item.Key, row, DestroyAllNeighbours);
                    m_InitTilesBeingAnimated.Add(m_Grid[item.Key, row]);
                    m_Grid[item.Key, row].InitPlayMoveDownAnimation(m_Grid.GetLength(1), m_Distance, InitOnTileFinishedFalling);
                    j++;
                }
            }
            
        }

        void SetAllTilesInteractable(bool interactable)
        {
            for (int i = 0; i < m_Grid.GetLength(0); i++)
            {
                for (int j = 0; j < m_Grid.GetLength(1); j++)
                {
                    m_Grid[i, j].SetInteractable(interactable);
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