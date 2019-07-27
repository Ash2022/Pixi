using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.EventSystems;

public class MouseSimulator : MonoBehaviour
{
   

    public static MouseSimulator Instance;

    

    public void Button_AutoStart()
    {
        ManagerView.Instance.Button_StartClicked();
        StartCoroutine(StartClicks());
    }

    IEnumerator StartClicks()
    {
        int total_game = 0;
        int total_score = 0;
        Time.timeScale = 10;

        while(total_game<30)
        {
            while (GameController.Instance.Game_active)
            {

                int total = ManagerView.Instance.Balls_holder.transform.childCount;

                GameObject go = ManagerView.Instance.Balls_holder.transform.GetChild(UnityEngine.Random.Range(0, total)).gameObject;

                if (go.GetComponent<BallView>() != null)
                {
                    go.GetComponent<BallView>().Button_Clicked();
                }
                else
                {
                    go.GetComponent<PowerView>().Button_Clicked();
                }

                yield return new WaitForSeconds(0.5f);
                ManagerView.Instance.ShowHideAvarageScore(false, "nothing");
            }

            total_game++;
            total_score += GameController.Instance.Score;
            ManagerView.Instance.AutoStartStarts();

            ManagerView.Instance.ShowHideAvarageScore(true, "Playing Game: " + total_game.ToString());

        }

        total_score = total_score / 30;

        ManagerView.Instance.ShowHideAvarageScore(true,"AVARAGE SCORE: " + total_score.ToString());
        
    }

    

}
