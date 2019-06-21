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

    List<string> m_my_touching = new List<string>();

    public void SetData2(int bubble_type,Vector3 pos, Action<int,Vector2, BallView> bubble_pop_action)
    {
        m_bubble_type = bubble_type;
        m_image.color = GetColor(bubble_type);
        float size = GetSize(bubble_type);
        m_rect.localScale = new Vector3(size, size, size);
        m_rect.localPosition = pos;
        m_bubble_pop_action = bubble_pop_action;
        
    }

    public List<string> GetTouching()
    {
        return m_my_touching;
    }

    public void Button_Clicked()
    {
        m_bubble_pop_action(m_bubble_type,m_rect.localPosition,this);
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
            return 1f;
        else if (size == 1)
            return 1.25f;
        else if (size == 2)
            return 1.55f;
        else if (size == 3)
            return 1.85f;
        else
            return 5f;

    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.GetComponent<BallView>()!=null && other.gameObject.GetComponent<BallView>().m_bubble_type == m_bubble_type)
            m_my_touching.Add(other.gameObject.name);
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<BallView>() != null && other.gameObject.GetComponent<BallView>().m_bubble_type == m_bubble_type)
            m_my_touching.Remove(other.gameObject.name);
    }
    

}
