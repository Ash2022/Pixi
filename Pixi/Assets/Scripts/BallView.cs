using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallView : MonoBehaviour
{
    [SerializeField] Image          m_image;
    [SerializeField] RectTransform  m_rect;
    int m_bubble_type;
    Action<int,Vector2, BallView> m_bubble_pop_action;
    
    public delegate bool bubble_action(int bubble_type, Vector2 position, BallView ballView);

    bubble_action m_bubble_pop_action2;

    bool m_no_touch = false;

    List<string> m_my_touching = new List<string>();

    public void SetData2(int bubble_type,Vector3 pos, bubble_action bubble_pop_action)
    {
        m_bubble_type = bubble_type;
        m_image.color = GetColor(bubble_type);
        float width = GetSize(bubble_type);
        
        m_rect.sizeDelta = new Vector2(width, width);

        gameObject.GetComponent<CircleCollider2D>().radius = width / 2;

        m_rect.localPosition = pos;
        m_bubble_pop_action2 = bubble_pop_action;
        
    }


    public List<BallView> BallsInMyRangeAndMatchingColor()
    {
        List<BallView> m_in_range = new List<BallView>();

        foreach (Transform T in ManagerView.Instance.Balls_holder.transform)
        {
            if(T.gameObject.GetComponent<BallView>()!=null && T.gameObject.GetComponent<BallView>().m_bubble_type == m_bubble_type)
            {
                RectTransform other_rect = T.GetComponent<RectTransform>();

                //compare distancces and see if its in range based on both objects radius
                double xpower = (m_rect.localPosition.x - other_rect.localPosition.x) * (m_rect.localPosition.x - other_rect.localPosition.x);
                double ypower = (m_rect.localPosition.y - other_rect.localPosition.y) * (m_rect.localPosition.y - other_rect.localPosition.y);

                double distance = Math.Sqrt(xpower + ypower);

                float other_raidus = other_rect.rect.width / 2;
                float curr_radius = m_rect.rect.width / 2;

                //reduced 0.1 to not lose float calculation touching
                if (distance < other_raidus + curr_radius - 0.1f)
                    m_in_range.Add(T.gameObject.GetComponent<BallView>());
            }
            
        }

        return m_in_range;
    }


    public List<string> GetTouching()
    {
        return m_my_touching;
    }

    public List<BallView> GetTouching2()
    {
        return BallsInMyRangeAndMatchingColor();
    }

    void OnMouseOver()
    {
        if(Input.GetMouseButton(0) || Input.touchCount>0)
            Button_Clicked();
    }
       

    public void Button_Clicked()
    {
        if(m_bubble_pop_action2(m_bubble_type,m_rect.localPosition,this))
            DestroyImmediate(gameObject);
    }

    private Color GetColor(int col)
    {
        if (col == 0)
            return Color.blue;
        else if (col == 1)
            return Color.green;
        else if (col == 2)
            return Color.yellow;
        else if (col == 3)
            return Color.red;
        else
            return Color.white;

    }

    private float GetSize(int size)
    {
        if (size == 0)
            return 100f;
        else if (size == 1)
            return 120f;
        else if (size == 2)
            return 145f;
        else if (size == 3)
            return 165f;
        else
            return 5f;

    }


}
