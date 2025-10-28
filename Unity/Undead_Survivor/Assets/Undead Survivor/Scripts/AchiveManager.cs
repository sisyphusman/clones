using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchiveManager : MonoBehaviour
{
    public GameObject[] lockCharacter;
    public GameObject[] unlockCharacter;

    enum Achive { UnlockPotato, UnlockBean }
    Achive[] achives;

    private void Awake()
    {
        achives = (Achive[])Enum.GetValues(typeof(Achive));

        if (PlayerPrefs.HasKey("MyData"))
        {
            Init();
        }
    }

    private void Init()
    {
        // ����� �ڵ�
        PlayerPrefs.SetInt("MyData", 1);

        foreach (Achive achive in achives)
        {
            PlayerPrefs.SetInt(achive.ToString(), 1);
        }
    }

    private void Start()
    {
        unLockCharacter();
    }

    void unLockCharacter()
    {
        for (int index = 0; index < lockCharacter.Length; index++)
        {
            string achiveName = achives[index].ToString();
            
            // ������ �޼��ߴ��� üũ
            bool isUnlock = PlayerPrefs.GetInt(achiveName) == 1;

            lockCharacter[index].SetActive(!isUnlock);
            unlockCharacter[index].SetActive(isUnlock);
        }
    }
}
