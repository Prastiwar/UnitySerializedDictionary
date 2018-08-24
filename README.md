# Unity-Serialized Dictionary

***Serialize Dictionary and see them visually in inspector***

[![GitHub last commit](https://img.shields.io/github/last-commit/Prastiwar/UnitySerializedDictionary.svg?label=Updated&style=flat-square&longCache=true)](https://github.com/Prastiwar/UnitySerializedDictionary/commits/master)
[![license](https://img.shields.io/github/license/Prastiwar/UnitySerializedDictionary.svg?style=flat-square&longCache=true)](https://github.com/Prastiwar/UnitySerializedDictionary/blob/master/LICENSE)
[![GitHub forks](https://img.shields.io/github/forks/Prastiwar/UnitySerializedDictionary.svg?style=social&label=Fork&longCache=true)](https://github.com/Prastiwar/UnitySerializedDictionary/fork)
[![GitHub stars](https://img.shields.io/github/stars/Prastiwar/UnitySerializedDictionary.svg?style=social&label=★Star&longCache=true)](https://github.com/Prastiwar/UnitySerializedDictionary/stargazers)
[![GitHub watchers](https://img.shields.io/github/watchers/Prastiwar/UnitySerializedDictionary.svg?style=social&labelWatcher&longCache=true)](https://github.com/Prastiwar/UnitySerializedDictionary/watchers)
[![GitHub contributors](https://img.shields.io/github/contributors/Prastiwar/UnitySerializedDictionary.svg?style=social&longCache=true)](https://github.com/Prastiwar/UnitySerializedDictionary/contributors)

![GitHub repo size in bytes](https://img.shields.io/github/repo-size/Prastiwar/UnitySerializedDictionary.svg?style=flat-square&longCache=true)
[![GitHub issues](https://img.shields.io/github/issues/Prastiwar/UnitySerializedDictionary.svg?style=flat-square&longCache=true)](https://github.com/Prastiwar/UnitySerializedDictionary/issues)
[![GitHub closed issues](https://img.shields.io/github/issues-closed/Prastiwar/UnitySerializedDictionary.svg?style=flat-square&longCache=true)](https://github.com/Prastiwar/UnitySerializedDictionary/issues)
[![GitHub pull requests](https://img.shields.io/github/issues-pr/Prastiwar/UnitySerializedDictionary.svg?style=flat-square&longCache=true)](https://github.com/Prastiwar/UnitySerializedDictionary/pulls)
[![GitHub closed pull requests](https://img.shields.io/github/issues-pr-closed/Prastiwar/UnitySerializedDictionary.svg?style=flat-square&longCache=true)](https://github.com/Prastiwar/UnitySerializedDictionary/pulls)

[![Made with Unity](https://img.shields.io/badge/Made%20with-Unity-000000.svg?longCache=true&style=for-the-badge&colorA=666677&colorB=222222)](https://unity3d.com/)

## Before start

- [x] Make sure you have at least **Unity 2017** (previous versions weren't tested)


## Getting Started

Take a look at examples in package [THERE](https://github.com/Prastiwar/UnitySerializedDictionary/blob/master/UnityDictionary.unitypackage) 


## Using

To serialize dictionary you need to write short wrapper:
```cs
[Serializable]
public class UDictionaryStringInt : UDictionary<string, int> { }
```  
and use it freely in your script
```cs
[SerializeField] private UDictionaryStringInt serializedDictionary;
```
If you want it to be drawed in inspector, you need to assign attribute directly to [UDictionaryDrawer](https://github.com/Prastiwar/UnitySerializedDictionary/blob/master/Editor/UDictionaryDrawer.cs) or add to its wrapper as you can see [THERE](https://github.com/Prastiwar/UnitySerializedDictionary/blob/master/Editor/UDictionaryDrawers.cs)

## Contributing

You can freely contribute with me by reporting issues and making pull requests!  
Please read [CONTRIBUTING.md](https://github.com/Prastiwar/UnitySerializedDictionary/blob/master/.github/CONTRIBUTING.md) for details on contributing.

## Authors

* ![Avatar](https://avatars3.githubusercontent.com/u/33370172?s=40&v=4)  [**Tomasz Piowczyk**](https://github.com/Prastiwar) - *The Creator*  
See also the list of [contributors](https://github.com/Prastiwar/UnitySerializedDictionary/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](https://github.com/Prastiwar/UnitySerializedDictionary/blob/master/LICENSE) file for details.

![Screenshot](https://i.imgur.com/FPVAmK1.png)
![Screenshot](https://i.imgur.com/QajAnlU.png)
