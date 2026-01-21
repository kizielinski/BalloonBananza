using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class PlayerAnimatorBuilder
{
    private const string ControllerAssetPath = "Assets/Settings/Player.controller";
    private const string ClipsFolderPath = "Assets/Settings/Animations/Player";
    // Put only the desired player sprites under this root (and subfolders)
    private const string SpritesRoot = "Assets/Sprites/Characters/Balloon";
    private static readonly Dictionary<string, string> DirectionFolders = new Dictionary<string, string>
    {
        { "Idle", "Idle" },
        { "Right", "Right" },
        { "Left", "Left" },
        { "Up", "Up" },
        { "Down", "Down" },
        { "RightUp", "RightUp" },
        { "RightDown", "RightDown" },
        { "LeftUp", "LeftUp" },
        { "LeftDown", "LeftDown" }
    };

    [MenuItem("Tools/Player/Build Animator Controller")]
    public static void BuildAnimatorController()
    {
        EnsureFolders();

        // Create or load controller
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerAssetPath);
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerAssetPath);
        }

        // Parameters
        EnsureParameter(controller, "Direction", AnimatorControllerParameterType.Int);
        EnsureParameter(controller, "IsMoving", AnimatorControllerParameterType.Bool);

        // Create clips from sprites (only under SpritesRoot)
        var clips = CreateDirectionClips();

        // Layers and states
        var layer = controller.layers.Length > 0 ? controller.layers[0] : null;
        if (layer == null)
        {
            controller.AddLayer("Base Layer");
            layer = controller.layers[0];
        }
        var sm = layer.stateMachine;

        var stateIdle = EnsureState(controller, sm, "Idle", clips.TryGetValue("Idle", out var idle) ? idle : null);
        var stateRight = EnsureState(controller, sm, "MoveRight", clips.TryGetValue("Right", out var right) ? right : null);
        var stateLeft = EnsureState(controller, sm, "MoveLeft", clips.TryGetValue("Left", out var left) ? left : null);
        var stateUp = EnsureState(controller, sm, "MoveUp", clips.TryGetValue("Up", out var up) ? up : null);
        var stateDown = EnsureState(controller, sm, "MoveDown", clips.TryGetValue("Down", out var down) ? down : null);
        var stateRightUp = EnsureState(controller, sm, "MoveRightUp", clips.TryGetValue("RightUp", out var rup) ? rup : null);
        var stateRightDown = EnsureState(controller, sm, "MoveRightDown", clips.TryGetValue("RightDown", out var rdown) ? rdown : null);
        var stateLeftUp = EnsureState(controller, sm, "MoveLeftUp", clips.TryGetValue("LeftUp", out var lup) ? lup : null);
        var stateLeftDown = EnsureState(controller, sm, "MoveLeftDown", clips.TryGetValue("LeftDown", out var ldown) ? ldown : null);

        // Set default state
        sm.defaultState = stateIdle;

        // Clear existing transitions on relevant states to avoid duplicates
        ClearTransitions(stateIdle);
        ClearTransitions(stateRight);
        ClearTransitions(stateLeft);
        ClearTransitions(stateUp);
        ClearTransitions(stateDown);
        ClearTransitions(stateRightUp);
        ClearTransitions(stateRightDown);
        ClearTransitions(stateLeftUp);
        ClearTransitions(stateLeftDown);

        // Transitions: Idle -> moving directions
        AddTransition(stateIdle, stateRight, new[]
        {
            ("IsMoving", AnimatorConditionMode.If, 0f),
            ("Direction", AnimatorConditionMode.Equals, 0f)
        });
        AddTransition(stateIdle, stateLeft, new[]
        {
            ("IsMoving", AnimatorConditionMode.If, 0f),
            ("Direction", AnimatorConditionMode.Equals, 1f)
        });
        AddTransition(stateIdle, stateUp, new[]
        {
            ("IsMoving", AnimatorConditionMode.If, 0f),
            ("Direction", AnimatorConditionMode.Equals, 2f)
        });
        AddTransition(stateIdle, stateDown, new[]
        {
            ("IsMoving", AnimatorConditionMode.If, 0f),
            ("Direction", AnimatorConditionMode.Equals, 3f)
        });
        AddTransition(stateIdle, stateRightUp, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 4f) });
        AddTransition(stateIdle, stateRightDown, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 5f) });
        AddTransition(stateIdle, stateLeftUp, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 6f) });
        AddTransition(stateIdle, stateLeftDown, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 7f) });

        // Transitions: between move states when direction changes
        var moveStates = new[]
        {
            (stateRight, 0f),
            (stateLeft, 1f),
            (stateUp, 2f),
            (stateDown, 3f),
            (stateRightUp, 4f),
            (stateRightDown, 5f),
            (stateLeftUp, 6f),
            (stateLeftDown, 7f)
        };
        foreach (var from in moveStates)
        {
            foreach (var to in moveStates)
            {
                if (from.Equals(to)) continue;
                AddTransition(from.Item1, to.Item1, new[]
                {
                    ("IsMoving", AnimatorConditionMode.If, 0f),
                    ("Direction", AnimatorConditionMode.Equals, to.Item2)
                });
            }
            // Move -> Idle
            AddTransition(from.Item1, stateIdle, new[]
            {
                ("IsMoving", AnimatorConditionMode.IfNot, 0f)
            });
        }

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Attach to selected GameObject if it has an Animator
        var go = Selection.activeGameObject;
        if (go != null)
        {
            var animator = go.GetComponent<Animator>();
            if (animator == null) animator = go.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            EditorUtility.SetDirty(animator);
        }

        Debug.Log("Player Animator Controller built and attached (if a GameObject was selected).");
    }

    [MenuItem("Tools/Player/Build Animator Controller From Selection")]
    public static void BuildAnimatorControllerFromSelection()
    {
        EnsureFolders();

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerAssetPath);
        if (controller == null)
        {
            controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerAssetPath);
        }

        EnsureParameter(controller, "Direction", AnimatorControllerParameterType.Int);
        EnsureParameter(controller, "IsMoving", AnimatorControllerParameterType.Bool);

        var clips = CreateDirectionClipsFromSelection();

        var layer = controller.layers.Length > 0 ? controller.layers[0] : null;
        if (layer == null)
        {
            controller.AddLayer("Base Layer");
            layer = controller.layers[0];
        }
        var sm = layer.stateMachine;

        var stateIdle = EnsureState(controller, sm, "Idle", clips.TryGetValue("Idle", out var idle) ? idle : null);
        var stateRight = EnsureState(controller, sm, "MoveRight", clips.TryGetValue("Right", out var right) ? right : null);
        var stateLeft = EnsureState(controller, sm, "MoveLeft", clips.TryGetValue("Left", out var left) ? left : null);
        var stateUp = EnsureState(controller, sm, "MoveUp", clips.TryGetValue("Up", out var up) ? up : null);
        var stateDown = EnsureState(controller, sm, "MoveDown", clips.TryGetValue("Down", out var down) ? down : null);
        var stateRightUp = EnsureState(controller, sm, "MoveRightUp", clips.TryGetValue("RightUp", out var rup) ? rup : null);
        var stateRightDown = EnsureState(controller, sm, "MoveRightDown", clips.TryGetValue("RightDown", out var rdown) ? rdown : null);
        var stateLeftUp = EnsureState(controller, sm, "MoveLeftUp", clips.TryGetValue("LeftUp", out var lup) ? lup : null);
        var stateLeftDown = EnsureState(controller, sm, "MoveLeftDown", clips.TryGetValue("LeftDown", out var ldown) ? ldown : null);

        sm.defaultState = stateIdle;
        ClearTransitions(stateIdle);
        ClearTransitions(stateRight);
        ClearTransitions(stateLeft);
        ClearTransitions(stateUp);
        ClearTransitions(stateDown);
        ClearTransitions(stateRightUp);
        ClearTransitions(stateRightDown);
        ClearTransitions(stateLeftUp);
        ClearTransitions(stateLeftDown);

        AddTransition(stateIdle, stateRight, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 0f) });
        AddTransition(stateIdle, stateLeft, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 1f) });
        AddTransition(stateIdle, stateUp, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 2f) });
        AddTransition(stateIdle, stateDown, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 3f) });
        AddTransition(stateIdle, stateRightUp, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 4f) });
        AddTransition(stateIdle, stateRightDown, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 5f) });
        AddTransition(stateIdle, stateLeftUp, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 6f) });
        AddTransition(stateIdle, stateLeftDown, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, 7f) });

        var moveStates = new[] { (stateRight, 0f), (stateLeft, 1f), (stateUp, 2f), (stateDown, 3f), (stateRightUp, 4f), (stateRightDown, 5f), (stateLeftUp, 6f), (stateLeftDown, 7f) };
        foreach (var from in moveStates)
        {
            foreach (var to in moveStates)
            {
                if (ReferenceEquals(from.Item1, to.Item1)) continue;
                AddTransition(from.Item1, to.Item1, new[] { ("IsMoving", AnimatorConditionMode.If, 0f), ("Direction", AnimatorConditionMode.Equals, to.Item2) });
            }
            AddTransition(from.Item1, stateIdle, new[] { ("IsMoving", AnimatorConditionMode.IfNot, 0f) });
        }

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        var go = Selection.activeGameObject;
        if (go != null)
        {
            var animator = go.GetComponent<Animator>() ?? go.AddComponent<Animator>();
            animator.runtimeAnimatorController = controller;
            EditorUtility.SetDirty(animator);
        }

        Debug.Log("Player Animator Controller built from selection and attached (if a GameObject was selected).");
    }

    private static void EnsureFolders()
    {
        CreateFolderIfMissing("Assets/Settings");
        CreateFolderIfMissing("Assets/Settings/Animations");
        CreateFolderIfMissing(ClipsFolderPath);
    }

    private static void CreateFolderIfMissing(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            var parent = Path.GetDirectoryName(path).Replace("\\", "/");
            var name = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, name);
        }
    }

    private static void EnsureParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
    {
        if (!controller.parameters.Any(p => p.name == name))
        {
            controller.AddParameter(name, type);
        }
    }

    private static AnimatorState EnsureState(AnimatorController controller, AnimatorStateMachine sm, string stateName, Motion motion)
    {
        var state = sm.states.FirstOrDefault(s => s.state.name == stateName).state;
        if (state == null)
        {
            state = sm.AddState(stateName);
        }
        state.motion = motion;
        return state;
    }

    private static void ClearTransitions(AnimatorState state)
    {
        foreach (var t in state.transitions.ToArray())
        {
            state.RemoveTransition(t);
        }
    }

    private static void AddTransition(AnimatorState from, AnimatorState to, IEnumerable<(string, AnimatorConditionMode, float)> conditions)
    {
        var t = from.AddTransition(to);
        t.hasExitTime = false;
        t.duration = 0f;
        foreach (var (param, mode, threshold) in conditions)
        {
            t.AddCondition(mode, threshold, param);
        }
    }

    private static Dictionary<string, AnimationClip> CreateDirectionClips()
    {
        var result = new Dictionary<string, AnimationClip>();
        var groups = new Dictionary<string, List<Sprite>>
        {
            { "Idle", new List<Sprite>() },
            { "Right", new List<Sprite>() },
            { "Left", new List<Sprite>() },
            { "Up", new List<Sprite>() },
            { "Down", new List<Sprite>() },
            { "RightUp", new List<Sprite>() },
            { "RightDown", new List<Sprite>() },
            { "LeftUp", new List<Sprite>() },
            { "LeftDown", new List<Sprite>() }
        };

        // Prefer folder-based selection: Assets/Sprites/Player/<Direction>
        foreach (var kvp in DirectionFolders)
        {
            var dirKey = kvp.Key;
            var folderPath = Path.Combine(SpritesRoot, kvp.Value).Replace("\\", "/");
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                var sprites = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath })
                    .Select(g => AssetDatabase.GUIDToAssetPath(g))
                    .Select(p => AssetDatabase.LoadAssetAtPath<Sprite>(p))
                    .Where(s => s != null)
                    .ToList();
                groups[dirKey].AddRange(sprites);
            }
        }

        // If no directional folders or empty, fallback to keyword scan under SpritesRoot only
        if (!groups.Values.Any(list => list.Count > 0))
        {
            var sprites = AssetDatabase.FindAssets("t:Sprite", new[] { SpritesRoot })
                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                .Select(p => AssetDatabase.LoadAssetAtPath<Sprite>(p))
                .Where(s => s != null)
                .ToList();
            foreach (var s in sprites)
            {
                var dirKey = GetDirectionKey(s.name);
                if (dirKey != null) groups[dirKey].Add(s);
            }
        }

        foreach (var kv in groups)
        {
            var key = kv.Key;
            var sprites = kv.Value.OrderBy(s => s.name).ToList();
            if (sprites.Count == 0) continue; // Skip if none found

            var clip = new AnimationClip();
            clip.frameRate = 12f;
            var binding = EditorCurveBinding.PPtrCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite");
            var keys = new ObjectReferenceKeyframe[sprites.Count];
            for (int i = 0; i < sprites.Count; i++)
            {
                keys[i] = new ObjectReferenceKeyframe
                {
                    time = i / clip.frameRate,
                    value = sprites[i]
                };
            }
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

            var clipPath = Path.Combine(ClipsFolderPath, $"Player_{key}.anim").Replace("\\", "/");
            AssetDatabase.CreateAsset(clip, clipPath);
            result[key] = clip;
        }

        AssetDatabase.SaveAssets();
        return result;
    }

    private static Dictionary<string, AnimationClip> CreateDirectionClipsFromSelection()
    {
        var result = new Dictionary<string, AnimationClip>();

        // Collect sprites from selection: supports Sprite, Texture2D (sliced), and folders
        var spriteSet = new HashSet<Sprite>();
        foreach (var obj in Selection.objects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) continue;

            if (obj is Sprite s)
            {
                spriteSet.Add(s);
                continue;
            }
            if (obj is Texture2D)
            {
                var reps = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                foreach (var rep in reps)
                {
                    if (rep is Sprite ss) spriteSet.Add(ss);
                }
                continue;
            }
            if (AssetDatabase.IsValidFolder(path))
            {
                var guids = AssetDatabase.FindAssets("t:Sprite", new[] { path });
                foreach (var g in guids)
                {
                    var sp = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GUIDToAssetPath(g));
                    if (sp != null) spriteSet.Add(sp);
                }
            }
        }

        var groups = new Dictionary<string, List<Sprite>>
        {
            { "Idle", new List<Sprite>() },
            { "Right", new List<Sprite>() },
            { "Left", new List<Sprite>() },
            { "Up", new List<Sprite>() },
            { "Down", new List<Sprite>() },
            { "RightUp", new List<Sprite>() },
            { "RightDown", new List<Sprite>() },
            { "LeftUp", new List<Sprite>() },
            { "LeftDown", new List<Sprite>() }
        };

        foreach (var sp in spriteSet)
        {
            var dirKey = GetDirectionKey(sp.name);
            if (dirKey != null) groups[dirKey].Add(sp);
        }

        foreach (var kv in groups)
        {
            var key = kv.Key;
            var sprites = kv.Value.OrderBy(s => s.name).ToList();
            if (sprites.Count == 0) continue;

            var clipPath = Path.Combine(ClipsFolderPath, $"Player_{key}.anim").Replace("\\", "/");
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (clip == null)
            {
                clip = new AnimationClip();
                clip.frameRate = 12f;
                AssetDatabase.CreateAsset(clip, clipPath);
            }
            clip.frameRate = 12f;
            var binding = EditorCurveBinding.PPtrCurve(string.Empty, typeof(SpriteRenderer), "m_Sprite");
            var keys = new ObjectReferenceKeyframe[sprites.Count];
            for (int i = 0; i < sprites.Count; i++)
            {
                keys[i] = new ObjectReferenceKeyframe { time = i / clip.frameRate, value = sprites[i] };
            }
            AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
            EditorUtility.SetDirty(clip);
            result[key] = clip;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return result;
    }

    private static string GetDirectionKey(string name)
    {
        var n = name.ToLowerInvariant();
        // Diagonals first to avoid matching cardinals inside them
        if ((n.Contains("right") && n.Contains("up")) || n.Contains("upright") || n.Contains("rightup")) return "RightUp";
        if ((n.Contains("right") && n.Contains("down")) || n.Contains("downright") || n.Contains("rightdown")) return "RightDown";
        if ((n.Contains("left") && n.Contains("up")) || n.Contains("upleft") || n.Contains("leftup")) return "LeftUp";
        if ((n.Contains("left") && n.Contains("down")) || n.Contains("downleft") || n.Contains("leftdown")) return "LeftDown";
        if (n.Contains("idle")) return "Idle";
        if (n.Contains("right") || n.Contains("east")) return "Right";
        if (n.Contains("left") || n.Contains("west")) return "Left";
        if (n.Contains("up") || n.Contains("north")) return "Up";
        if (n.Contains("down") || n.Contains("south")) return "Down";
        return null;
    }
}
