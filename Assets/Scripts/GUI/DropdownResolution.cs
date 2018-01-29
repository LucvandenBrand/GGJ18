using UnityEngine;
using UnityEngine.UI;

/* UI dropdown menu item that allows you to set the screen resolution. */
public class DropdownResolution : Dropdown
{

	/* On start, add all possible resolutions and add the event listener. */
	protected override void Start ()
    {
        base.Start();
        Resolution curResolution = Screen.currentResolution;
        int optionNumber = 0;
        Resolution[] resolutions = Screen.resolutions;
        options.Clear();

        foreach (Resolution resolution in resolutions)
        {
            string label = resolution.width + "x" + resolution.height;
            options.Add(new OptionData(label));
            if (resolution.Equals(curResolution))
                this.value = optionNumber;
            optionNumber++;
        }

        this.onValueChanged.AddListener(delegate 
        {
            Resolution resolution = resolutions[this.value];
            Screen.SetResolution(resolution.width, resolution.height, 
                                  Screen.fullScreen, resolution.refreshRate);
        });
    }
}
