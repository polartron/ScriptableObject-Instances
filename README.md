# ScriptableObject-Instances
Based on the Unite Austin 2017 talk by Ryan Hipple.
https://www.youtube.com/watch?v=raQ3iHhE_Kk

## Description
ScriptableObject-Instances allows you to have runtime variables and events tied to unique instances of your prefabs. This is an extension built upon the original implementation by Ryan Hipple where you could only have shared global ScriptableObjects at the project level.

![Example Flow](https://i.imgur.com/OWPdgei.png)

![Variable Types](https://i.imgur.com/X08VBnb.png)
## Features

* Global Scriptable Object Variables
* Global Scriptable Object Events
* Instanced Scriptable Object Variables
* Instanced Scriptable Object Events

* Clamped Scriptable Object References
* Expression based Scriptable Object References
* Added/Subtracted/Multiplied/Divided Scriptable Object References
* Code Generator

## Installation

### Manual Installation
- Download the repository and put the files in a folder named **com.fasteraune.scriptableobjectinstances** in your **Packages** folder located next to your **Assets** folder
- Add the following to your **manifest.json** file in your **Packages** folder 
> "com.fasteraune.scriptableobjectinstances": "file:com.fasteraune.scriptableobjectinstances"
- Follow the https://docs.unity3d.com/Manual/ScriptCompilationAssemblyDefinitionFiles.html workflow and add a reference to **Fasteraune.SO.Instances.asmdef**

## Usage

A complete example can be found in the Example folder

## Code Generation Example

The following is an example of an editor script you can add to your own project in order to generate the files you need. 

```
using System;
using System.IO;
using Fasteraune.SO.Instances.Editor;
using UnityEditor;
using UnityEngine;

public class VariableGenerator
{
    [MenuItem("Assets/Variables/Generate")]
    static void Generate()
    {
        Type[] generateTypes =
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

        string path = Path.Combine(Application.dataPath, "Scripts/Generated/");

        ScriptGenerator.GenerateScripts(generateTypes, path);
    }
}
```
