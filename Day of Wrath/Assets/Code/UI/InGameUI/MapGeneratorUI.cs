using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapGeneratorUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button mapGeneratorPerlinButton;
    public Button mapGeneratorSimplexButton;
    public Button flattenTerrainButton;
    public Button generateUsingCheckboxesButton;

    public Slider widthSlider;
    public Slider lengthSlider;
    public Slider intensitySlider;

    public TextMeshProUGUI widthValueText;
    public TextMeshProUGUI lengthValueText;
    public TextMeshProUGUI intensityValueText;

    public Toggle perlinToggle;
    public Toggle simplexToggle;
    public Toggle hillToggle;
    public Toggle valleyToggle;
    public Toggle craterToggle;
    public Toggle mountainToggle;

    public Slider hillIntensitySlider;
    public Slider valleyIntensitySlider;
    public Slider craterIntensitySlider;
    public Slider mountainIntensitySlider;

    [Header("Generator Reference")]
    public MapGenerator mapGenerator;

    private void Start()
    {
        mapGeneratorPerlinButton.onClick.AddListener(GenerateMapUsingPerlinNoise);
        mapGeneratorSimplexButton.onClick.AddListener(GenerateMapUsingSimplexNoise);
        flattenTerrainButton.onClick.AddListener(FlattenTerrain);
        generateUsingCheckboxesButton.onClick.AddListener(GenerateUsingCheckboxes);

        widthSlider.onValueChanged.AddListener(OnWidthChanged);
        lengthSlider.onValueChanged.AddListener(OnHeightChanged);
        intensitySlider.onValueChanged.AddListener(OnIntensityChanged);

        widthSlider.value = mapGenerator.terrainWidth;
        lengthSlider.value = mapGenerator.terrainLength;
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

    public void GenerateUsingCheckboxes()
    {
        mapGenerator.GenerateFromCheckboxes(
            perlinToggle.isOn,
            simplexToggle.isOn,
            hillToggle.isOn,
            valleyToggle.isOn,
            craterToggle.isOn,
            mountainToggle.isOn,
            hillIntensitySlider.value,
            valleyIntensitySlider.value,
            craterIntensitySlider.value,
            mountainIntensitySlider.value
        );
    }

    private void OnWidthChanged(float value)
    {
        mapGenerator.terrainWidth = Mathf.RoundToInt(value);
        UpdateLabels();
    }

    private void OnHeightChanged(float value)
    {
        mapGenerator.terrainLength = Mathf.RoundToInt(value);
        UpdateLabels();
    }

    private void OnIntensityChanged(float value)
    {
        mapGenerator.heightMultiplier = value;
        UpdateLabels();
    }

    private void UpdateLabels()
    {
        if (widthValueText != null) widthValueText.text = mapGenerator.terrainWidth.ToString();
        if (lengthValueText != null) lengthValueText.text = mapGenerator.terrainLength.ToString();
        if (intensityValueText != null) intensityValueText.text = mapGenerator.heightMultiplier.ToString("F1");
    }
}
