using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





public class ManagerView : MonoBehaviour
{

    public enum BubbleColors
    {
        Red,
        Green,
        Blue,
        Purple,
        Pink,
        Yellow,
        Turquize,
        Black

    }


    public static ManagerView Instance;
    

    [SerializeField] GameObject m_ball_prefab;
    [SerializeField] GameObject m_powerup_prefab;

    [SerializeField] GameObject m_balls_holder;
    [SerializeField] GameObject m_powerup_holder;
    [SerializeField] Text m_score_text;
    [SerializeField] Image m_bar_fill;
    [SerializeField] GameObject m_start_button;
    [SerializeField] GameObject m_start_auto_button;
    [SerializeField] GameObject m_game_over;
    [SerializeField] GameObject m_avarage_score;
    [SerializeField] Text m_avarage_score_label;
    GameUtils m_utils;
    
    int bubble_index = 0;
 

    public GameObject Balls_holder { get => m_balls_holder; set => m_balls_holder = value; }
    
    public GameUtils Utils { get => m_utils; set => m_utils = value; }
    public Image Bar_fill { get => m_bar_fill; set => m_bar_fill = value; }
    public Text Score_text { get => m_score_text; set => m_score_text = value; }

    private void Awake()
    {
        Instance = this;
        m_utils = GetComponent<GameUtils>();
    }

    public void StartGame()
    {
        ResetAll();
                
        m_start_button.SetActive(false);
        m_start_auto_button.SetActive(false);
        m_game_over.SetActive(false);
    }
    
    public void AutoStartStarts()
    {
        StartGame();
        //StartCoroutine(GameTimer());
    }

    public void Button_StartClicked()
    {/*
        Time.timeScale = 1;
        StartGame();
        StartCoroutine(GameTimer());*/
    }


    private void ResetAll()
    {
        m_avarage_score.SetActive(false);
        bubble_index = 0;

        foreach (Transform t in m_balls_holder.transform)
        {
            Destroy(t.gameObject);
        }

        foreach (Transform t in m_powerup_holder.transform)
        {
            Destroy(t.gameObject);
        }

    }

    public IEnumerator GenerateBalls(int amount,Vector2 bubble_size, float delay, bool from_top = false)
    {
        int counter = 0;
        while (counter < amount)
        {
            GenerateRandomBubble(from_top, bubble_size);
            counter++;
            yield return new WaitForSeconds(delay);
        }
    }

    public void GenerateRandomBubble(bool from_top, Vector2 bubble_size)
    {
        Vector2 pos = new Vector2(UnityEngine.Random.Range(-100, 100), 0);
        
        if (from_top)
            pos = new Vector2(UnityEngine.Random.Range(-100, 100), 200f);

        GameObject new_bubble = Instantiate(m_ball_prefab, m_balls_holder.GetComponent<Transform>());

        new_bubble.transform.SetAsLastSibling();

        new_bubble.name = "Bubble" + bubble_index;

        bubble_index++;

        int bubble_type = UnityEngine.Random.Range(0, 4);
        int size = UnityEngine.Random.Range((int)bubble_size.x, (int)bubble_size.y);

        new_bubble.GetComponent<BallView>().SetData2((BubbleColors)bubble_type, size, pos, GameController.Instance.BubblePopped);
    }

    public void GeneratePowerUp(int power_up_type, Vector2 pos)
    {
        GameObject new_power_up = Instantiate(m_powerup_prefab, m_powerup_holder.GetComponent<Transform>());
        new_power_up.transform.SetAsLastSibling();
    
        if(power_up_type==1)
        {
            new_power_up.GetComponent<PowerView>().SetData1(1, pos,GameController.Instance.Bomb,null);
        }
        else if(power_up_type==2)
        {
            new_power_up.GetComponent<PowerView>().SetData1(2, pos,null, GameController.Instance.HealthUp);
        }
        else
        {
            new_power_up.GetComponent<PowerView>().SetData1(3, pos,null, GameController.Instance.KillAll);
        }
    }

    public bool InList(string name)
    {
        foreach (Transform t in m_balls_holder.transform)
        {
            if (t.name == name)
                return true;
        }
        return false;
    }
    
    public void KillObject(string name)
    {
        foreach (Transform t in m_balls_holder.transform)
        {
            if (t.name == name)
            {
                DestroyImmediate(t.gameObject);
                GenerateRandomBubble(true, GameController.Instance.GetCurrLevel().GetSizeRange());
            }
  
        }
    }


    public void AddMyTouchingRecursive(BallView ballView,List<string> touching)
    {
        if(ballView.GetTouching2().Count == 0)
        {
            return;
        }
        else
        {
            List<BallView> ballViews = ballView.GetTouching2();

            for (int i = 0; i < ballViews.Count; i++)
            {
                if(touching.Contains(ballViews[i].gameObject.name) ==false) 
                {
                    touching.Add(ballViews[i].gameObject.name);
                    AddMyTouchingRecursive(ballViews[i], touching);
                }
            }
        }
    }
    

    public void ShowHideAvarageScore(bool show,string text)
    {
        m_avarage_score.SetActive(show);

        m_avarage_score_label.text = text;
    }
    

    public void TimerExpired()
    {
        m_start_button.SetActive(true);
        m_start_auto_button.SetActive(true);
        m_game_over.SetActive(true);
    }


}
