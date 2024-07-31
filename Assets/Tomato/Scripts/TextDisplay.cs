using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TextDisplay : MonoBehaviour
{
    public Text displayText; // 在Inspector中分配UI Text组件
    private string[] lines; // 存储文本行
    private int currentLineIndex = 0; // 当前行索引

    void Start()
    {
        // 读取文本文件
        #if UNITY_EDITOR
        string filePath = Path.Combine(Application.dataPath, "../Build/show.txt"); // 获取与exe同一层级的路径
        #else
        string filePath = Path.Combine(Application.dataPath, "../show.txt"); // 获取与exe同一层级的路径
        #endif
        if (File.Exists(filePath))
        {
            lines = File.ReadAllLines(filePath); // 读取所有行
            StartCoroutine(DisplayText()); // 启动协程
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }

    private IEnumerator DisplayText()
    {
        while (true)
        {
            if (lines != null && lines.Length > 0)
            {
                displayText.text = lines[currentLineIndex]; // 显示当前行
                currentLineIndex = (currentLineIndex + 1) % lines.Length; // 循环索引
            }
            yield return new WaitForSeconds(60); // 等待60秒
        }
    }
}