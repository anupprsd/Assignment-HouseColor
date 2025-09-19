using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColourPicker : MonoBehaviour
{
    public float currentHue, currentSat, currentVal;

    // Keeps track if the game is in gameplay mode or UI mode
    public static GameState state = GameState.GAME;

    // UI elements
    [SerializeField] private RawImage hueImage, satValImage, outputImage;
    [SerializeField] private Slider hueslider;
    [SerializeField] private TMP_InputField hexInputField;
    [SerializeField] GameObject colorPicker;
    [SerializeField] SVImageControll svImageControll;

    private MeshRenderer selectedWall;   
    private Texture2D hueTexture, svTexture, outputTexture; 

    private void Start()
    {
        // Create initial textures and update the preview
        CreateHueImage();
        CreateSVImage();
        CreateOutputImage();
        UpdateOutputImage();
    }

    // Creates the hue selection strip 
    private void CreateHueImage()
    {
        hueTexture = new Texture2D(1, 16);
        hueTexture.wrapMode = TextureWrapMode.Clamp;
        hueTexture.name = "HueTexture";

        // Fill texture vertically with hue values
        for (int i = 0; i < hueTexture.height; i++)
            hueTexture.SetPixel(0, i, Color.HSVToRGB((float)i / hueTexture.height, 1, 1f));

        hueTexture.Apply();

        currentHue = 0;
        hueImage.texture = hueTexture;
    }

    // Creates the Saturation Value square for given hue
    private void CreateSVImage()
    {
        svTexture = new Texture2D(16, 16);
        svTexture.wrapMode = TextureWrapMode.Clamp;
        svTexture.name = "SatValTexture";

        // Fill with colors ranging from 0 to 1
        for (int y = 0; y < svTexture.height; y++)
            for (int x = 0; x < svTexture.width; x++)
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));

        svTexture.Apply();
        currentSat = 0;  // default saturation
        currentVal = 1;  // default value/brightness

        satValImage.texture = svTexture;
    }

    // Creates the preview strip for final output color
    private void CreateOutputImage()
    {
        outputTexture = new Texture2D(1, 16);
        outputTexture.wrapMode = TextureWrapMode.Clamp;
        outputTexture.name = "OutputTexture";

        Color currentColour = Color.HSVToRGB(currentHue, currentSat, currentVal);

        // Fill with the currently selected color
        for (int i = 0; i < outputTexture.height; i++)
            outputTexture.SetPixel(0, i, currentColour);

        outputTexture.Apply();
        outputImage.texture = outputTexture;
    }

    // Updates the preview color and applies it to selected wall
    private void UpdateOutputImage()
    {
        Color currentColour = Color.HSVToRGB(currentHue, currentSat, currentVal);

        for (int i = 0; i < outputTexture.height; i++)
            outputTexture.SetPixel(0, i, currentColour);

        outputTexture.Apply();

        // Update selected wall material if any
        // Use instance material as there are multiple walls and there could be different colors to them. 
        // SharedMaterial would apply the same color to every wall
        if (selectedWall != null)
            selectedWall.material.SetColor("_BaseColor", currentColour);
    }

    // Called when user picks new Saturation and Value
    public void SetSV(float S, float V)
    {
        currentSat = S;
        currentVal = V;
        UpdateOutputImage();
    }

    // Updates the SV texture when user changes hue
    public void UpdateSVImage(float hue)
    {
        currentHue = hue;

        // Recalculate SV image with new hue
        for (int y = 0; y < svTexture.height; y++)
            for (int x = 0; x < svTexture.width; x++)
                svTexture.SetPixel(x, y, Color.HSVToRGB(currentHue, (float)x / svTexture.width, (float)y / svTexture.height));

        svTexture.Apply();
        UpdateOutputImage();
    }

    // Called when user selects a wall object to paint
    public void ChangeColorOfObject(GameObject go)
    {
        // Play selection animation
        go.transform.GetChild(0).GetComponent<Animation>().Play();

        // Set the selected wall reference
        selectedWall = go.GetComponent<MeshRenderer>();

        // Load wall’s current color into the picker
        LoadColorOfWall(go);

        // Show picker UI
        ShowUI();
    }

    // Reads wall current material color into HSV sliders/UI
    private void LoadColorOfWall(GameObject go)
    {
        Color lastColor = go.GetComponent<MeshRenderer>().material.color;
        Color.RGBToHSV(lastColor, out currentHue, out currentSat, out currentVal);

        hueslider.value = currentHue;
        svImageControll.SetPickerPosition(currentSat, currentVal);
        UpdateSVImage(currentHue);
    }

    // Opens color picker UI
    public void ShowUI()
    {
        colorPicker.SetActive(true);
        Cursor.lockState = CursorLockMode.None; // Unlock cursor for UI
        state = GameState.UI;
    }

    // Closes color picker UI
    public void CloseUI()
    {
        colorPicker.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor back for gameplay
        state = GameState.GAME;
    }
}

// Enum to track whether in gameplay or UI mode
public enum GameState
{
    GAME,
    UI
}
