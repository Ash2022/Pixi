using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;





public class ManagerView : MonoBehaviour
{
    
    public static ManagerView Instance;
    

    [SerializeField] GameObject m_ball_prefab;
    [SerializeField] GameObject m_powerup_prefab;

    [SerializeField] GameObject m_balls_holder;
    [SerializeField] Text m_score_text;
    [SerializeField] Image m_bar_fill;
    [SerializeField] GameObject m_start_button;
    [SerializeField] GameObject m_start_auto_button;
    [SerializeField] GameObject m_game_over;
    [SerializeField] GameObject m_avarage_score;
    [SerializeField] Text m_avarage_score_label;

    int m_last_popped_bubble_type = -1;
    int m_bubbles_in_combo = 0;

    int bubble_index = 0;

    float m_health = 1000;
    float m_decrease_rate = 50;

    bool m_game_active = false;
    bool m_special_power_up = false;
    bool m_mouse_went_up = false;

    const float INITIAL_DECREASE_RATE = 50;

    const float TOTAL_FULL_TIME = 20f;

    int m_score = 0;

    public GameObject Balls_holder { get => m_balls_holder; set => m_balls_holder = value; }
    public bool Game_active { get => m_game_active; set => m_game_active = value; }
    public int Score { get => m_score; set => m_score = value; }

    private void Awake()
    {
        Instance = this;
        
    }

    public void AutoStartStarts()
    {
        StartGame();
        StartCoroutine(GameTimer());
    }

    public void Button_StartClicked()
    {
        Time.timeScale = 1;
        StartGame();
        StartCoroutine(GameTimer());
    }

    public void StartGame()
    {
        ResetAll();

        StartCoroutine(GenerateBalls(7, 0.1f));
        m_start_button.SetActive(false);
        m_start_auto_button.SetActive(false);
        m_game_over.SetActive(false);
    }

    private void ResetAll()
    {
        m_avarage_score.SetActive(false);
        bubble_index = 0;
        m_last_popped_bubble_type = -1;
        m_bubbles_in_combo = 0;

        foreach (Transform t in m_balls_holder.transform)
        {
            Destroy(t.gameObject);
        }

        m_game_active = true;
        m_health = 1000;
        UpdateScore(0, true);
    }

    private void UpdateScore(int added_to_score, bool reset = false)
    {
        if (reset)
            m_score = 0;
        else
            m_score += added_to_score;

        //Debug.Log("Score Added: " + added_to_score);

        m_score_text.text = "SCORE: " + m_score;

    }

    IEnumerator GenerateBalls(int amount, float delay, bool from_top = false)
    {
        int counter = 0;
        while (counter < amount)
        {
            GenerateRandomBubble(from_top);
            counter++;
            yield return new WaitForSeconds(delay);
        }
    }

    private void GenerateRandomBubble(bool from_top)
    {
        Vector2 pos = new Vector2(UnityEngine.Random.Range(-100, 100), 0);

        if (from_top)
            pos = new Vector2(UnityEngine.Random.Range(-100, 100), 500f);

        GameObject new_bubble = Instantiate(m_ball_prefab, m_balls_holder.GetComponent<Transform>());

        new_bubble.name = "Bubble" + bubble_index;

        bubble_index++;

        int bubble_type = UnityEngine.Random.Range(0, 4);
        
        new_bubble.GetComponent<BallView>().SetData2(bubble_type, pos, BubblePopped);
    }

    private void GeneratePowerUp(int power_up_type, Vector2 pos)
    {
        GameObject new_power_up = Instantiate(m_powerup_prefab, m_balls_holder.GetComponent<Transform>());

        if(power_up_type==1)
        {
            new_power_up.GetComponent<PowerView>().SetData1(1, pos, Bomb);
        }
        else if(power_up_type==2)
        {
            new_power_up.GetComponent<PowerView>().SetData1(2, pos, HealthUp);
        }
        else
        {
            new_power_up.GetComponent<PowerView>().SetData1(3, pos, KillAll);
        }
    }

    private bool InList(string name)
    {
        foreach (Transform t in m_balls_holder.transform)
        {
            if (t.name == name)
                return true;
        }
        return false;
    }
    
    private void KillObject(string name)
    {
        foreach (Transform t in m_balls_holder.transform)
        {
            if (t.name == name)
            {
                Destroy(t.gameObject);
                GenerateRandomBubble(true);
            }
                
        }
    }

    private void Bomb(List<string> colliders_in_range)
    {
        for (int i = 0; i < colliders_in_range.Count; i++)
        {
            //might be that the same bubble is inside many colliders
            if (InList(colliders_in_range[i]))
            {
                KillObject(colliders_in_range[i]);
                
            }
            
                
        }
    }

    private void HealthUp(List<string> colliders_in_range)
    {
        m_health += 20;
    }

    private void KillAll(List<string> colliders_in_range)
    {
        m_special_power_up = true;
    }

    private void AddMyTouchingRecursive(BallView ballView,List<string> touching)
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
    

    public bool BubblePopped(int bubble_type, Vector2 pos, BallView ballView)
    {
        if(m_special_power_up)
        {
            List<string> vs = new List<string>();

            AddMyTouchingRecursive(ballView, vs);

            for (int i = 0; i < vs.Count; i++)
            {
                KillObject(vs[i]);
            }

            m_special_power_up = false;

        }
        else
        {
            if (bubble_type == m_last_popped_bubble_type)
            {
                m_bubbles_in_combo++;
                
            }
            else
                m_bubbles_in_combo = 0;

            

            if (m_bubbles_in_combo == 0 && m_mouse_went_up==false)
            {
                //need to put something totally new in the last poped bubble so it wont go in the condition next frame as a combo
                m_last_popped_bubble_type = -1;
                //will not kill the bubble
                return false;
            }
                
            else
            {
                m_last_popped_bubble_type = bubble_type;
                m_mouse_went_up = false;
                GenerateRandomBubble(true);

                HandlePowerUps(pos);

                int add_to_score = 10;

                if (m_bubbles_in_combo >= 3)
                    add_to_score += (m_bubbles_in_combo) * 2;

                UpdateScore(add_to_score);
                return true;
            }

            
        }
        return false;

    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButton(0) == false)
            m_mouse_went_up = true;

#elif UNITY_ANDROID
        if (Input.touchCount == 0)
            m_mouse_went_up = true;
#endif
        
    }

    private void HandlePowerUps(Vector2 pos)
    {
        if(UnityEngine.Random.Range(0,1f)>0.8f)
        {
            float new_range = UnityEngine.Random.Range(0, 1f);

            //Debug.Log("new_range " + new_range);

            if(new_range<0.3f)
            {
                GeneratePowerUp(1, pos);
            }
            else if(new_range>=0.3f && new_range<0.9f)
            {
                GeneratePowerUp(2, pos);
            }
            else if(new_range>=0.9)
            {
                GeneratePowerUp(3, pos);
            }
        }
    }

    public void ShowHideAvarageScore(bool show,string text)
    {
        m_avarage_score.SetActive(show);

        m_avarage_score_label.text = text;

    }
    
    

    IEnumerator GameTimer()
    {
        float start_time = Time.time;
        float total_time = m_health / m_decrease_rate;
        float time_left = total_time - (Time.time - start_time);

        Debug.Log(total_time);
        Debug.Log(time_left);

        while (time_left > 0)
        {

            int time_passed = Convert.ToInt32((Time.time - start_time) / 5);

            m_decrease_rate = INITIAL_DECREASE_RATE + time_passed * 5;

            m_health -= Time.deltaTime * m_decrease_rate;

            m_health = Math.Min(m_health, TOTAL_FULL_TIME * INITIAL_DECREASE_RATE);

            time_left = m_health / m_decrease_rate;

            m_bar_fill.fillAmount = time_left / TOTAL_FULL_TIME;

            yield return null;
        }

        m_game_active = false;
        m_start_button.SetActive(true);
        m_start_auto_button.SetActive(true);
        m_game_over.SetActive(true);
    }




}
