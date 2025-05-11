using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapGeneratorUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button mapGeneratorPerlinButton;
    public Button mapGeneratorSimplexButton;
    public Button flattenTerrainButton;

    public Slider widthSlider;
    public Slider heightSlider;
    public Slider intensitySlider;

    public TextMeshProUGUI widthValueText;
    public TextMeshProUGUI heightValueText;
    public TextMeshProUGUI intensityValueText;

    [Header("Generator Reference")]
    public MapGenerator mapGenerator;

    private void Start()
    {
        mapGeneratorPerlinButton.onClick.AddListener(GenerateMapUsingPerlinNoise);
        mapGeneratorSimplexButton.onClick.AddListener(GenerateMapUsingSimplexNoise);
        flattenTerrainButton.onClick.AddListener(FlattenTerrain);

        widthSlider.onValueChanged.AddListener(OnWidthChanged);
        heightSlider.onValueChanged.AddListener(OnHeightChanged);
        intensitySlider.onValueChanged.AddListener(OnIntensityChanged);

        widthSlider.value = mapGenerator.width;
        heightSlider.value = mapGenerator.height;
        intensitySlider.value = mapGenerator.heightMultiplier;

        UpdateLabels();
    }

    private void GenerateMapUsingPerlinNoise()
    {
        mapGenerator.GenerateWithPerlin();
    }

    private void GenerateMapUsingSimplexNoise()
    {
        mapGenerator.GenerateWithSimplex();
    }

    private void FlattenTerrain()
    {
        mapGenerator.FlattenTerrain();
    }

    private void OnWidthChanged(float value)
    {
        mapGenerator.width = Mathf.RoundToInt(value);
        UpdateLabels();
    }

    private void OnHeightChanged(float value)
    {
        mapGenerator.height = Mathf.RoundToInt(value);
        UpdateLabels();
    }

    private void OnIntensityChanged(float value)
    {
        mapGenerator.heightMultiplier = value;
        UpdateLabels();
    }

    private void UpdateLabels()
    {
        if (widthValueText != null) widthValueText.text = mapGenerator.width.ToString();
        if (heightValueText != null) heightValueText.text = mapGenerator.height.ToString();
        if (intensityValueText != null) intensityValueText.text = mapGenerator.heightMultiplier.ToString("F1");
    }
}
