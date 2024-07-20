using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockItem : ChiMonoBehaviour
{
    public int value;
    public int Value => value;

    [SerializeField] bool isClickBlock = false;

    public Position position;
    public Position Position => position;

    [SerializeField] private ManagerGame managerGame;

    protected override void Awake()
    {
        managerGame = FindObjectOfType<ManagerGame>();
        GetComponent<Button>().onClick.AddListener(CheckBLock);
    }

  



    public void CheckBLock()
    {
        if(managerGame != null)
        {
            isClickBlock = true;
            managerGame.AddBlock(this);
        }
    }

   


    public void SetValue(int value)
    {
        this.value = value;
    }

    public void setPosition(Position pos)
    {
        this.position = pos;
    } 
}
