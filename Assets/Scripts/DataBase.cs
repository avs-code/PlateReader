using System.IO;
using UnityEngine;
using TMPro;
using System.Collections;

public class DataBase : MonoBehaviour
{
    public TextRecognitionSaver textRecognitionSaver;
    public TextMeshProUGUI infoText; // Referencia al componente TextMeshPro en el Canvas
    private string jsonFilePath = "Assets/DB/plateDB.json";

    void Start()
    {
        // Verificar si textRecognitionSaver está asignado
        if (textRecognitionSaver == null)
        {
            Debug.LogError("TextRecognitionSaver no está asignado.");
            return;
        }

        // Verificar si infoText está asignado
        if (infoText == null)
        {
            Debug.LogError("InfoText no está asignado.");
            return;
        }

        // Suscribirse al evento OnMatriculaUpdated
        textRecognitionSaver.OnMatriculaUpdated += OnMatriculaUpdated;

        // Iniciar la corrutina para esperar la matrícula
        StartCoroutine(WaitForMatricula());
    }

    private IEnumerator WaitForMatricula()
    {
        // Esperar hasta que la matrícula no sea nula o vacía
        while (string.IsNullOrEmpty(textRecognitionSaver.GetMatricula()))
        {
            yield return null; // Esperar un frame
        }

        // Leer el valor de "matricula" desde TextRecognitionSaver
        string matricula = textRecognitionSaver.GetMatricula();
        Debug.Log("DataBase.cs Matrícula leída: " + matricula);

        // Leer el archivo JSON
        string jsonContent = File.ReadAllText(jsonFilePath);
        //Debug.Log("Contenido del JSON: " + jsonContent); // Log para comprobar los datos del JSON

        MatriculaInfo matriculaInfo = JsonUtility.FromJson<MatriculaInfo>(jsonContent);

        // Verificar si la matrícula leída coincide con la del archivo JSON
        if (matriculaInfo.matricula == matricula)
        {
            //Debug.Log("Matrícula: " + matriculaInfo.matricula);
            //Debug.Log("Autorizado: " + matriculaInfo.autorizado);
            //Debug.Log("Año de Matriculación: " + matriculaInfo.anoDeMatriculacion);

            // Actualizar el Text con la información del JSON
            infoText.text = $"Matrícula: {matriculaInfo.matricula}\n" +
                            $"Autorizado: {matriculaInfo.autorizado}\n" +
                            $"Año de Matriculación: {matriculaInfo.anoDeMatriculacion}";
        }
        else
        {
            //Debug.LogWarning("La matrícula no coincide con la del archivo JSON.");
            infoText.text = "La matrícula no coincide con la del archivo JSON.";
        }
    }

    private void OnMatriculaUpdated()
    {
        // Iniciar la corrutina para actualizar la información de la matrícula
        StartCoroutine(WaitForMatricula());
    }

    void OnDestroy()
    {
        // Desuscribirse del evento OnMatriculaUpdated
        if (textRecognitionSaver != null)
        {
            textRecognitionSaver.OnMatriculaUpdated -= OnMatriculaUpdated;
        }
    }
}

[System.Serializable]
public class MatriculaInfo
{
    public string matricula;
    public string autorizado;
    public int anoDeMatriculacion;
}
