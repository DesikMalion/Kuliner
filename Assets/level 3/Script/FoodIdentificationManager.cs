
using UnityEngine;
using TMPro;

public class FoodIdentificationManager : MonoBehaviour
{
    [Header("Daftar Semua Bahan")]
    public FoodIngredient[] ingredients;

    [Header("UI")]
    public TMP_Text feedbackText;
    public TMP_Text progressText;
public GameObject finishButton;
    [Header("Pesan")]
    public string correctMessage = "Benar! Bahan sudah ditempatkan.";
    public string wrongMessage = "Salah! Periksa kategori dan kondisi bahan.";
    public string completedMessage =
        "Semua bahan berhasil diidentifikasi!";

    private int correctCount;
    private int totalIngredients;

    private void Start()
    {
        ResetSimulation();
    }

    public void CheckIngredient(
        FoodIngredient ingredient,
        FoodSortingArea.SortingAreaType targetArea)
    {
        if (ingredient == null)
            return;

        // Jangan menghitung bahan yang sudah benar dua kali.
        if (ingredient.isCorrectlySorted)
            return;

        bool isCorrect = false;

        // Bahan rusak harus masuk area BahanRusak,
        // apa pun kategori bahan tersebut.
        if (ingredient.condition ==
            FoodIngredient.FoodCondition.Rusak)
        {
            isCorrect =
                targetArea ==
                FoodSortingArea.SortingAreaType.BahanRusak;
        }
        else
        {
            // Bahan segar harus masuk area sesuai kategorinya.
            switch (ingredient.category)
            {
                case FoodIngredient.FoodCategory.Nabati:
                    isCorrect =
                        targetArea ==
                        FoodSortingArea.SortingAreaType.Nabati;
                    break;

                case FoodIngredient.FoodCategory.Hewani:
                    isCorrect =
                        targetArea ==
                        FoodSortingArea.SortingAreaType.Hewani;
                    break;
            }
        }

        if (isCorrect)
        {
            ingredient.MarkAsSorted();
            correctCount++;

            ShowFeedback(
                correctMessage + "\n" +
                ingredient.ingredientName
            );

            UpdateProgress();

            if (correctCount >= totalIngredients)
{
    ShowFeedback(completedMessage);

    if (finishButton != null)
        finishButton.SetActive(true);

    Debug.Log("Identifikasi bahan selesai!");
}
        }
        else
        {
            string expectedArea =
                GetExpectedArea(ingredient);

            ShowFeedback(
                wrongMessage + "\n" +
                ingredient.ingredientName +
                "\nSeharusnya: " + expectedArea
            );

            Debug.LogWarning(
                ingredient.ingredientName +
                " salah ditempatkan. Seharusnya: " +
                expectedArea
            );
        }
    }

    private string GetExpectedArea(FoodIngredient ingredient)
    {
        if (ingredient.condition ==
            FoodIngredient.FoodCondition.Rusak)
        {
            return "Bahan Rusak";
        }

        return ingredient.category ==
            FoodIngredient.FoodCategory.Nabati
            ? "Nabati"
            : "Hewani";
    }

    private void UpdateProgress()
    {
        if (progressText != null)
        {
            progressText.text =
                "Bahan selesai: " +
                correctCount + "/" + totalIngredients;
        }
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;

        Debug.Log(message);
    }

    public void ResetSimulation()
    {
           if (finishButton != null)
        finishButton.SetActive(false);
        correctCount = 0;
        totalIngredients = 0;

        if (ingredients != null)
        {
            foreach (FoodIngredient ingredient in ingredients)
            {
                if (ingredient == null)
                    continue;

                ingredient.ResetIngredient();
                totalIngredients++;
            }
        }

        UpdateProgress();

        ShowFeedback(
            "Ambil bahan dari tempat penyimpanan, periksa kondisinya, " +
            "lalu letakkan pada area yang sesuai."
        );
    }
}
