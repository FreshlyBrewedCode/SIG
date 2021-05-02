# SIG

SIG is a Unity editor tool/framework for **S**ynthetic **I**mage dataset **G**eneration using a node-based workflow. SIG was developed in the scope of my 2021 bachelor thesis "Synthetic image dataset generation for the training of deep learning models using node-based workflows" at the "Hochschule Hannover" in the course of studies "Mediendesigninformatik".

## Repository Structure

This repository is a Unity project that serves as a monorepo for all available SIG sub-packages. The repo can be cloned and opened with Unity 2020.2 or above. The individual sub-packages are located in the `Packages` directory.

## Sub-Packages

| Sub-Package                        | Directory                               | Dependencies          |
| ---------------------------------- | --------------------------------------- | --------------------- |
| [`SIG.Core`][Core]                 | `Packages/de.frebreco.SIG.Core`         |                       |
| [`SIG.Python`][Python]             | `Packages/de.frebreco.SIG.Python`       | Core                  |
| [`SIG.Dataset`][Dataset]           | `Packages/de.frebreco.SIG.Dataset`      | Core                  |
| [`SIG.Dataset.Coco`][Dataset.Coco] | `Packages/de.frebreco.SIG.Dataset.Coco` | Core, Dataset, Python |

## Installation

If you intend to make changes to the SIG codebase you should fork and clone this repository. If you only want to use SIG's editor features and access exposed API's without touching the codebase you can install SIG using the Unity package manager (UPM):

### OpenUPM (Recommended)

SIG can be installed from the [OpenUPM][OpenUPM] registry. SIG can be added using the OpenUPM CLI, however, it is easier to edit your `manifest.json` file directly.

1. Navigate to the root directory of your Unity project and open the `Packages/manifest.json` file. Add a `scopedRegistries` entry for OpenUPN (if you already have an entry for OpenUPM make sure the `scopes` below are included):
```json
  "scopedRegistries": [
    {
        "name": "package.openupm.com",
        "url": "https://package.openupm.com",
        "scopes": [
            "de.frebreco.SIG",
            "com.alelievr.node-graph-processor",
            "jillejr.newtonsoft.json-for-unity",
            "com.openupm"
        ]
    }
  ]
```
2. Add all of the required SIG sub-packages to the `dependencies` property of your `Packages/manifest.json` file:
```json
    "dependencies": {
        "de.frebreco.SIG.Core": "1.0.0",
        "de.frebreco.SIG.Python": "1.0.0",
        "de.frebreco.SIG.Dataset": "1.0.0",
        "de.frebreco.SIG.Dataset.Coco": "1.0.0"
    }
```


[Core]: /Packages/de.frebreco.SIG.Core/README.md
[Python]: /Packages/de.frebreco.SIG.Python/README.md
[Dataset]: /Packages/de.frebreco.SIG.Dataset/README.md
[Dataset.Coco]: /Packages/de.frebreco.SIG.Dataset.Coco/README.md

[OpenUPM]: https://openupm.com/