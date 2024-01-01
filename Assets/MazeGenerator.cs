using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    private const int UP = 1, RIGHT = 2, DOWN = 4, LEFT = 8;
    public int numRows = 10, numCols = 5;
    public Sprite[] wallSprites;
    public Sprite floorSprite;
    private float[] probStopping = {0f, 0f, 0.3f, 0.7f, 1f};
    private float[] probBranchStopping = {0f, 0.5f, 1f};
    private void Start() {
        int[,] cells = new int[numRows, numCols];
        setGhostSpawn(cells);
        generateMaze(cells);
        int[,] map = cellToMap(cells);
        renderMap(map, wallSprites, floorSprite);
    }

    private static void setGhostSpawn(int[,] cells)
    {
        int mid = cells.GetLength(0) / 2;
        cells[mid, 0] = RIGHT + UP + LEFT;
        cells[mid, 1] = UP + LEFT;
        cells[mid - 1, 0] = RIGHT + DOWN + LEFT;
        cells[mid - 1, 1] = DOWN + LEFT;
    }

    private void generateMaze(int[,] cells)
    {
        float[] probStopping = {0f, 0f, 0.3f, 0.7f, 1f};
        float[] probBranchStopping = {0f, 0.5f, 1f};
        while (true) {
            List<Tuple<int, int>> candidateLeftCells = leftMostCells(cells);
            foreach (Tuple<int, int> cell in candidateLeftCells) print(cell.Item1 + " " + cell.Item2);
            if (candidateLeftCells.Count == 0) break;
            Tuple<int, int> center = candidateLeftCells[UnityEngine.Random.Range(0, candidateLeftCells.Count - 1)];
            Tuple<int, int> lastCell = center;
            Tuple<int, int> currentCell = center;
            bool branching = false;
            int size = 0;
            while (true) {
                if ((branching && probBranchStopping[size] <= UnityEngine.Random.Range(0f, 1f)) || probStopping[size] <= UnityEngine.Random.Range(0f, 1f)) break;
                if ((branching && size > 2) || size > 4) break;
                List<int> candidateOpenCells = adjacentOpenCells(cells, currentCell);
                size++;
                if (candidateOpenCells.Count == 0) {
                    if (currentCell.Equals(lastCell)) break;
                    currentCell = lastCell;
                    size = 0;
                    branching = true;
                    continue;
                }
                lastCell = currentCell;
                int dir = candidateOpenCells[UnityEngine.Random.Range(0, candidateOpenCells.Count - 1)];
                switch (dir) {
                    case UP:
                        currentCell = new Tuple<int, int>(currentCell.Item1 + 1, currentCell.Item2);
                        cells[currentCell.Item1, currentCell.Item2] += DOWN;
                        cells[currentCell.Item1 - 1, currentCell.Item2] += UP;
                        break;
                    case DOWN:
                        currentCell = new Tuple<int, int>(currentCell.Item1 - 1, currentCell.Item2);
                        cells[currentCell.Item1, currentCell.Item2] += UP;
                        cells[currentCell.Item1 + 1, currentCell.Item2] += DOWN;
                        break;
                    case LEFT:
                        currentCell = new Tuple<int, int>(currentCell.Item1, currentCell.Item2 - 1);
                        cells[currentCell.Item1, currentCell.Item2] += RIGHT;
                        cells[currentCell.Item1, currentCell.Item2 + 1] += LEFT;
                        break;
                    case RIGHT:
                        currentCell = new Tuple<int, int>(currentCell.Item1, currentCell.Item2 + 1);
                        cells[currentCell.Item1, currentCell.Item2] += LEFT;
                        cells[currentCell.Item1, currentCell.Item2 - 1] += RIGHT;
                        break;
                
                }
            }
        }
    }

    private static int[,] cellToMap(int[,] cells)
    {
        int numRows = cells.GetLength(0), numCols = cells.GetLength(1);
        int[,] map = new int[(numRows * 3) + 1, numCols * 3];
        for (int i = 0; i < numRows; i++)
            for (int j = 0; j < numCols; j++)
            {
                int bottomLeftx = j * 3, bottomLefty = i * 3;
                //Bottom and right edge are paths by default
                map[bottomLefty, bottomLeftx] = -1;
                map[bottomLefty, bottomLeftx + 1] = -1;
                map[bottomLefty, bottomLeftx + 2] = -1;
                map[bottomLefty + 1, bottomLeftx + 2] = -1;
                map[bottomLefty + 2, bottomLeftx + 2] = -1;
                switch (cells[i, j]) {
                    case 1:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = UP + DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = UP + DOWN + LEFT;
                        break;
                    case 2:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + LEFT + RIGHT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = DOWN + RIGHT + LEFT;
                        break;
                    case 3:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + LEFT + RIGHT;
                        map[bottomLefty + 2, bottomLeftx] = UP + DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = UP + DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = DOWN + RIGHT + LEFT;
                        break;
                    case 4:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + DOWN;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + LEFT;
                        break;
                    case 5:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + DOWN;
                        map[bottomLefty + 2, bottomLeftx] = UP + DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = UP + DOWN + LEFT;
                        break;
                    case 6:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + DOWN + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + LEFT + DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = DOWN + RIGHT + LEFT;
                        break;
                    case 7:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + RIGHT + LEFT;
                        map[bottomLefty, bottomLeftx + 2] = UP + DOWN + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + DOWN + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + LEFT + DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx] = UP + DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = UP + DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 2] = UP + DOWN + RIGHT + LEFT;
                        break;
                    case 8:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + LEFT;
                        break;
                    case 9:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT + LEFT + UP;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + LEFT + UP;
                        break;
                    case 10:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + RIGHT + LEFT;
                        break;
                    case 11:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + RIGHT + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 2] = UP + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT + LEFT + UP;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + RIGHT + LEFT + UP;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + RIGHT + LEFT + UP;
                        break;
                    case 12:
                        map[bottomLefty, bottomLeftx] = UP + RIGHT + DOWN;
                        map[bottomLefty, bottomLeftx + 1] = UP + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT + DOWN + LEFT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT + DOWN;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT + LEFT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + LEFT;
                        break;
                    default:
                        map[bottomLefty + 1, bottomLeftx] = UP + RIGHT;
                        map[bottomLefty + 1, bottomLeftx + 1] = UP + LEFT;
                        map[bottomLefty + 2, bottomLeftx] = DOWN + RIGHT;
                        map[bottomLefty + 2, bottomLeftx + 1] = DOWN + LEFT;
                        break;
                }
            }
        
        //Top row of map
        for (int i = 0; i < numCols * 3; i++) {
            map[map.GetLength(0) - 1, i] = -1;
        }
        
        int ghostSpawny = numRows * 3 / 2 - 2;
        //Below ghost spawn
        for (int i = 0; i < 6; i++)
            map[ghostSpawny - 1, i] = -1;

        //Right and left of ghost spawn
        for (int i = ghostSpawny; i < ghostSpawny + 5; i++)
            map[i, 5] = -1;

        //Inside host spawn
        for (int i = ghostSpawny + 1; i < ghostSpawny + 4; i++)
            for (int j = 0; j < 4; j++)
                map[i, j] = -1;

        //Walls of ghost spawn
        for (int i = ghostSpawny + 1; i < ghostSpawny + 4; i++)
            map[i, 4] = UP + DOWN;
        for (int i = 1; i < 4; i++) {
            map[ghostSpawny, i] = LEFT + RIGHT;
            map[ghostSpawny + 4, i] = LEFT + RIGHT;
        }
        //Corners of ghost spawn
        map[ghostSpawny, 0] = RIGHT;
        map[ghostSpawny + 4, 0] = RIGHT;
        map[ghostSpawny, 4] = LEFT + UP;
        map[ghostSpawny + 4, 4] = LEFT + DOWN;
        return map;
    }

    private static void renderMap(int[,] map, Sprite[] wallSprites, Sprite floorSprite)
    {
        int numRows = map.GetLength(0);
        int numCols = map.GetLength(1);
        int yOffset = (numRows - 1) / 2;
        for (int i = 0; i < numRows; i++)
            for (int j = 0; j < numCols; j++) {
                if (j == 0)
                    renderTile((map[i, j] == -1) ? floorSprite : wallSprites[map[i, j] + LEFT], j, i - yOffset);
                else {
                    renderTile((map[i, j] == -1) ? floorSprite : wallSprites[map[i, j]], j, i - yOffset);
                    renderTile((map[i, j] == -1) ? floorSprite : wallSprites[oppositeIndex(map[i, j])], -j, i - yOffset);
                }
            }
    }

    private List<Tuple<int, int>> leftMostCells(int[,] cells) {
        List<Tuple<int, int>> leftMostCells = new List<Tuple<int, int>>();
        for (int i = 0; i < numCols; i++){
            for (int j = 0; j < numRows; j++) {
                if (cells[j, i] == 0) leftMostCells.Add(new Tuple<int, int>(j, i));
            }
            if (leftMostCells.Count > 0) break;
        }
        return leftMostCells;
    }

    private static List<int> adjacentOpenCells(int[,] cells, Tuple<int, int> center)
    {
        List<int> adjacentOpenCells = new List<int>();
        int x = center.Item2, y = center.Item1;
        if (y > 0 && cells[y - 1, x] == 0) adjacentOpenCells.Add(DOWN);
        if (y < cells.GetLength(0) - 1 && cells[y + 1, x] == 0) adjacentOpenCells.Add(UP);
        if (x > 0 && cells[y, x - 1] == 0) adjacentOpenCells.Add(LEFT);
        if (x < cells.GetLength(1) - 1 && cells[y, x + 1] == 0) adjacentOpenCells.Add(RIGHT);
        return adjacentOpenCells;
    }

    private static void renderTile(Sprite sprite, int x, int y)
    {
        GameObject tile = new GameObject("Tile" + x + "," + y);
        tile.transform.position = new Vector3(x, y, 0);
        SpriteRenderer renderer = tile.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
    }

    private static int oppositeIndex(int index)
    {
        switch (index) {
            case RIGHT:
                return LEFT;
            case RIGHT + UP:
                return LEFT + UP;
            case RIGHT + DOWN:
                return LEFT + DOWN;
            case RIGHT + UP + DOWN:
                return LEFT + UP + DOWN;
            case LEFT:
                return RIGHT;
            case LEFT + UP:
                return RIGHT + UP;
            case LEFT + DOWN:
                return RIGHT + DOWN;
            case LEFT + UP + DOWN:
                return RIGHT + UP + DOWN;
            default:
                return index;
        }
    }
}