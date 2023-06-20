using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private HealthManager hManager;
    [SerializeField] private Transform gridRef;
    [SerializeField] private Sprite lifePic;
    public void Initialize(int value) 
    {
        for (int i = 0; i < value; i++)
        {
            GameObject newLife = new GameObject($"Life {i + 1}");
            Image newSprite = newLife.AddComponent<Image>();
            //set parent
            newLife.transform.SetParent(gridRef);

            newSprite.sprite = lifePic;
            newLife.transform.localScale = Vector3.one;

        }
        //add death counter to player's event
        //playerRef.Hurting += UpdateDeath;
    }

    public void UpdateDeath() 
    {
        Destroy(gridRef.GetChild(gridRef.childCount - 1).gameObject);
    }

    private void Start()
    {
        Initialize(hManager.lives);
    }

    private void OnEnable()
    {
        hManager.loseLife += UpdateDeath;
    }
    private void OnDisable()
    {
        hManager.loseLife -= UpdateDeath;
    }
}
