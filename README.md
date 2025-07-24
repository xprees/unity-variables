# Unity Variables Package - cz.xprees.variables

[![NPM Version](https://img.shields.io/npm/v/cz.xprees.variables)](https://www.npmjs.com/package/cz.xprees.variables)

This package contains a set of variables based on the ScriptableObjects,
which make it great for storing and simply sharing state without tight coupling.
Moreover, it provides a way to share variables (state) across multiple scenes making it powerful for multi-scene Unity project.

## Features

- **ScriptableObject Variables** - A set of variables that can be used to store and share state across scenes.
    - Simply extend the [`VariableBaseSO<T>`](Runtime/Base/VariableBaseSO.cs) class to create your own variable types, see examples in
      the [BoolVariable](Runtime/Primitive/BoolVariable.cs) folder.
    - Watchable Variables - You can subscribe to `onValueChanged` event to get notified when the value changes.
- **References** - A way to reference variables or inlined values in the Project (changeable in the Inspector).
    - You can directly set the value in the Inspector or reference a variable.
    - This makes flexible go to choice how to use the variables or inlined values in your scripts.
- **Variable Modifiers** - Wrapper classes that allow you to modify the value of the variable in a specific way.
    - For example, you can use the [`BoolModifier`](Runtime/Modifiers/BoolModifier.cs)
- **Variable Aggregations** - A set of classes that allow you to aggregate multiple variables into one.
    - For example, you can use the [`BoolAggregation`](Runtime/Aggregations/BoolAggregation.cs) to aggregate multiple boolean variables into one.

## Installation

Install the package using npm scoped registry in `Project Settings > Package Manager > Scoped Registries`

[Unity Docs - Install a UPM package from a Git URL](https://docs.unity3d.com/6000.1/Documentation/Manual/upm-ui-giturl.html)

```json
{
    "name": "NPM - xprees",
    "url": "https://registry.npmjs.org",
    "scopes": [
        "cz.xprees",
        "com.dbrizov.naughtyattributes"
    ]
}

```

Then simply install the package using the Unity Package Manager using the _NPM - xprees_ scope or by the package name `cz.xprees.variables`.
