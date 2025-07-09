using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class endingManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        async Task LoadMenu()
        {
            await Task.Delay(7000);

            SceneManager.LoadScene("Menu");
        }
        _ = LoadMenu();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
    }
}
