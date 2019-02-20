# WIP README


# ScriptableObject-Instanced-Variables
Based on the Unite Austin 2017 talk by Ryan Hipple. This version allows you to have instanced variables as well.
https://www.youtube.com/watch?v=raQ3iHhE_Kk

## Installation

### Manual Installation
- Download the repository and put the files in a folder named **com.fasteraune.scriptableobjectvariables** in your **Packages** folder located next to your **Assets** folder
- Add the following to your **manifest.json** file in your **Packages** folder 
> "com.fasteraune.scriptableobjectvariables": "file:com.fasteraune.scriptableobjectvariables"

## Usage
![Example](https://i.imgur.com/h6vTlFo.png)

See the Example folder

## Script Generation Example

```
using System;
using System.IO;
using Fasteraune.Variables.Editor;
using UnityEditor;
using UnityEngine;

public class VariableGenerator
{
    [MenuItem("Assets/Variables/Generate")]
    static void Generate()
    {
        Type[] types =
        {
            typeof(float),
            typeof(int),
            typeof(bool),
            typeof(string),
            
            typeof(Transform),
            typeof(Vector3),
            typeof(GameObject),
            typeof(AudioSource),
            typeof(AudioClip),
            typeof(Rigidbody)
        };

        string path = Path.Combine(Application.dataPath, "Scripts/Variables/Generated/");
        
        ScriptGenerator.GenerateScripts(types, path);
    }
}
```
