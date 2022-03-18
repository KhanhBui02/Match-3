using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    private Board board;
    private WaitForSeconds waitFindAllMatches = new WaitForSeconds(.2f);

    public List<GameObject> currentMaches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return waitFindAllMatches;
        for(int i = 0; i< board.width; i++)
        {
            for(int j = 0; j <board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                if(currentDot != null)
                {
                    if(i>0 && i<board.width - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if(leftDot != null && rightDot != null)
                        {
                            if(leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                if (!currentMaches.Contains(leftDot))
                                {
                                    currentMaches.Add(leftDot);
                                }
                                leftDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMaches.Contains(rightDot))
                                {
                                    currentMaches.Add(rightDot);
                                }
                                rightDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMaches.Contains(currentDot))
                                {
                                    currentMaches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < board.height - 1)
                    {
                        GameObject downDot = board.allDots[i, j - 1];
                        GameObject upDot = board.allDots[i, j + 1];
                        if (downDot != null && upDot != null)
                        {
                            if (downDot.tag == currentDot.tag && upDot.tag == currentDot.tag)
                            {
                                if (!currentMaches.Contains(downDot))
                                {
                                    currentMaches.Add(downDot);
                                }
                                downDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMaches.Contains(upDot))
                                {
                                    currentMaches.Add(upDot);
                                }
                                upDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMaches.Contains(currentDot))
                                {
                                    currentMaches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
