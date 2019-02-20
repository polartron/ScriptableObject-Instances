# WIP README


# ScriptableObject-Instanced-Variables
Based on the Unite Austin 2017 talk by Ryan Hipple. This version allows you to have instanced variables as well.
https://www.youtube.com/watch?v=raQ3iHhE_Kk

## Installation

### Manual Installalation
- Download the repository and put the files in a folder named **com.fasteraune.scriptableobjectvariables** in your **Packages** folder located next to your **Assets** folder
- Add the following to your **manifest.json** file in your **Packages** folder 
> "com.fasteraune.scriptableobjectvariables": "file:com.fasteraune.scriptableobjectvariables"

### Package Manager (2013.3.x +)
There is an experimental feature allowing you to reference the project by an url to the repository. Add the following to your **manifest.json** file in your **Packages** folder 
> "com.fasteraune.scriptableobjectvariables": "https://github.com/polartron/ScriptableObject-Instanced-Variables.git"

**Optional**

Add the following package to see and manage versions of the package https://github.com/mob-sakai/UpmGitExtension

## Usage

See the Example folder

## Adding Types

The package features a code generator that, based on an array of types, generates the scripts needed for your variables.

To add your own type. Edit **ReferenceScriptGenerator.cs** found in **Variables/Editor** and add an entry to the types array. 
![types](https://i.imgur.com/WgqWsZH.png)

If you are adding your own type from your own project, add a reference to your project assembly in the **Fasteraune.ScriptableObjectVariables.asmdef** file located in the root of the package.

To generate the scripts, use this menu option.
> Assets->Generate ScriptableObject Variable Scripts
