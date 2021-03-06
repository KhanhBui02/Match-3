using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}
public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject destroyEffect;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;

    private BackgroungTile[,] allTiles;
    private FindMatches findMatches;
    private WaitForSeconds waitDecreaseRow = new WaitForSeconds(.4f);
    private WaitForSeconds waitFillBoard = new WaitForSeconds(.5f);

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroungTile[width, height];
        allDots = new GameObject[width, height];
        Setup();
    }

    private void Setup()
    {
        for(int i=0; i<width;i++)
        {
            for(int j=0; j<height;j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet);
                GameObject backgroundTile =  Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name= "( " + i + ", " + j + " )";

                int dotToUse = Random.Range(0, dots.Length);

                int maxIteration = 0;
                while (MatchesAt(i, j, dots[dotToUse]) && maxIteration < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIteration++;
                    Debug.Log(maxIteration);
                }
                maxIteration = 0;

                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;

                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row >1)
        {
            if(allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            { 
                return true;
            }
        }
        else if(column <= 1 || row <=1)
        { 
            if (row > 1)
            {
                if(allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(allDots[column, row].GetComponent<Dot>().isMatched)
        {
            findMatches.currentMaches.Remove(allDots[column, row]);
            Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity); 
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for(int i =0; i< width;i++)
        {
            for(int j=0; j<height; j++)
            {
                if(allDots[i,j]!=null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return waitDecreaseRow;
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j]!=null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return waitFillBoard;

        while (MatchesOnBoard())
        {
            yield return waitFillBoard;
            DestroyMatches();
        }

        yield return waitFillBoard;
        currentState = GameState.move;
    }
}
