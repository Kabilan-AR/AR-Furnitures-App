using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    public GameObject menu;
    public GameObject loading;
    public GameObject home;

    public static int itemNumber;
    // Start is called before the first frame update
    void Start()
    {
        
        home.SetActive(true);
        loading.SetActive(false);
        menu.SetActive(false);
    }
    public void  menuCallBack()
    {
        menu.SetActive(false);
        loading.SetActive(false);
        home.SetActive(true);
    }
    public void homeCall()
    {
        home.SetActive(false );
        menu.SetActive(true );
        loading.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void itemSelected(int value)
    {
        itemNumber = value;
        StartCoroutine(delayLoadScene());
    }
    IEnumerator delayLoadScene()
    {
        menu.SetActive(false);
        loading.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(1);
    }
}
