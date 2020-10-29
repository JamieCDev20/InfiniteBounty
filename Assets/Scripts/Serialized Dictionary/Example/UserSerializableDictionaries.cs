using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SBDictionary : SerializableDictionary<string, bool> { }

[Serializable]
public class ActionIntDictionary : SerializableDictionary<Action, int> { }

[Serializable]
public class GoalIntDictionary : SerializableDictionary<Goal, int> { }