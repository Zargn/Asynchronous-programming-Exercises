using DoNotEdit_1;
using Exercise_1;

/*
 * In this Exercise, we're simulating the following case:
 * We have built GTA VI.
 * The Game spawns 10 AI Pedestrians, which all spawn in random locations and need to calculate some
 * smooth Path that makes them act naturally in their environment.
 * This calculation unfortunately is Performance-Heavy (it takes 2 seconds)
 * The player is not okay with the Game freezing for 20 or even 2 seconds.
 * That means, we need to implement the Path Request asynchronously, to allow it to run in a separate thread and run in parallel.
 * If we implement this nicely, it will mean that the AI won't move for two seconds at first (which we can cover up by spawning them far away), but other than that, the player won't even notice, that these expensive calculations had to happen.
 *
 * The knowledge learned here applies to any game that you make, where you need to call an expensive method and don't want your framerate to drop.
 */

namespace Exercise_1
{
    public class Unit
    {
        public IPath? _path;
        private bool calculating;
        
        public async Task Update()
        {
            // if there is no _path
            // calculate one using Program.CalculatePath()
            // if there is a path, call Run on it (every Update)
            if (_path == null && !calculating)
            {
                calculating = true;
                new Thread(() => CalculatePath()).Start();
            }

            _path.Run();
        }

        private void CalculatePath()
        {
            _path = Program.CalculatePath();
        }
    }
}



// DO NOT EDIT ANY OF THE CODE DOWN HERE!
namespace DoNotEdit_1
{
    using System.Diagnostics;
    public interface IPath
    {
        void Run();
    }

    public static class Program
    {
        private static volatile int pathsHaveBeenRequested;
        private static volatile int pathsHaveBeenRunSixtyTimes;
        private class Path : IPath
        {
            private int runCount = 0;
            public void Run()
            {
                runCount++;
                if (runCount != 60)
                    return;
                Interlocked.Increment(ref pathsHaveBeenRunSixtyTimes);
            }
        }
        public static IPath CalculatePath()
        {
            Interlocked.Increment(ref pathsHaveBeenRequested);
            Thread.Sleep(2000);
            return new Path();
        }
        static void Main()
        {
            int frameCount = 0;
            var stopWatch = Stopwatch.StartNew();
            var units = Enumerable.Range(0, 10).Select(i => new Unit()).ToArray();
            while (pathsHaveBeenRunSixtyTimes < units.Length)
            {
                Thread.Sleep(1000/60); // 60 fps
                foreach (var unit in units)
                {
                    unit.Update();
                }
                frameCount++;
            }
            stopWatch.Stop();
            
            Console.WriteLine($"It took the game {stopWatch.Elapsed} to calculate Paths on 10 Units and Run() them for 60 Frames. (Expected: ca. 3 seconds)");
            Console.WriteLine($"A total of {pathsHaveBeenRequested} Paths have been requested. (Expected: 10)");
            Console.WriteLine($"The game ran at ca. {frameCount/stopWatch.Elapsed.TotalSeconds} Frames per Second (Expected: 55+)");
        }
    }
}


// #####################################################################################################################


// using DoNotEdit_2;
// using Exercise_2;

/*
 * Here, we're simulating another common scenario:
 * We want to fluently switch from one scene to another.
 * One is already loaded and needs to be unloaded, another one needs to be loaded and presented.
 * We want this to happen behind the curtains of a loading screen, though, because levels just popping in and out don't look great.
 * Also, all of these actions happen asynchronously, because they take time.
 * Loading something from the hard drive takes a couple hundred milli-seconds.
 * And the Loading Screen also has some smooth fade in and fade out transitions.
 * 
 * You need to choose the correct sequence of these actions and wait for them to complete in a timely manner.
 * How fast can you make this transition happen? :)
 */

// namespace Exercise_2
// {
//     public static class SceneSwitcher
//     {
//         /// <summary>
//         /// This Method should smoothly transition from one Scene to another using a loading Screen.
//         /// It should only return, when the scenes are fully transitioned.
//         /// </summary>
//         /// <param name="oldScene">The scene that needs to be unloaded</param>
//         /// <param name="newScene">The scene that needs to be loaded and activated</param>
//         public static async Task SwitchScenesAsync(IScene oldScene, IScene newScene)
//         {
//             // Use Program.LoadLoadingScreenAsync() to create a loading screen
//             Program.LoadLoadingScreenAsync()
//         }
//     }
// }



// // DO NOT EDIT ANY OF THE CODE DOWN HERE!
// namespace DoNotEdit_2
// {
//     using System.Diagnostics;
//     public interface ILoadingScreen
//     {
//         /// <summary>
//         /// Shows the Loading Screen. Which hides loading and unloading happening behind it.
//         /// </summary>
//         /// <returns></returns>
//         Task ShowAsync();
//         /// <summary>
//         /// Hides the Loading Screen. Which shows the level again.
//         /// </summary>
//         /// <returns></returns>
//         Task HideAsync();
//     }
//
//     public interface IScene
//     {
//         /// <summary>
//         /// Loads the Scene from the Hard Drive to activate it later.
//         /// </summary>
//         /// <returns></returns>
//         Task LoadAsync();
//         /// <summary>
//         /// Activates a loaded Scene.
//         /// </summary>
//         /// <returns></returns>
//         Task ActivateAsync();
//         /// <summary>
//         /// Unloads a loaded or activated scene.
//         /// </summary>
//         /// <returns></returns>
//         Task UnloadAsync();
//         /// <summary>
//         /// This scene is now loaded and can be Activated.
//         /// </summary>
//         bool IsLoaded { get; }
//         /// <summary>
//         /// This scene is now visible to the player.
//         /// </summary>
//         bool IsActivated { get; }
//     }

//     public static class Program
//     {
//         private static volatile int pathsHaveBeenRequested;
//         private static volatile int pathsHaveBeenRunSixtyTimes;
//         private class LoadingScreen : ILoadingScreen
//         {
//             public static bool Shown { get; private set; }
//
//             public async Task ShowAsync()
//             {
//                 if (Shown)
//                 {
//                     throw new Exception("Cannot show Loading Screen. It's shown already.");
//                 }
//                 Console.WriteLine("Starting Show Loading Screen Animation");
//                 await Task.Delay(150);
//                 Shown = true;
//                 Console.WriteLine("Finished Show Loading Screen Animation");
//             }
//
//             public async Task HideAsync()
//             {
//                 if (!Shown)
//                 {
//                     throw new Exception("Cannot hide Loading Screen. It's hidden already.");
//                 }
//                 Console.WriteLine("Starting Hide Loading Screen Animation");
//                 await Task.Delay(150);
//                 Shown = false;
//                 Console.WriteLine("Finished Hide Loading Screen Animation");
//             }
//         }
//         public static async Task<ILoadingScreen> LoadLoadingScreenAsync()
//         {
//             Console.WriteLine("Loading Loading Screen...");
//             await Task.Delay(500);
//             Console.WriteLine("Loading Loading Screen Done.");
//             return new LoadingScreen();
//         }
//
//         private class Scene : IScene
//         {
//             private readonly string _name;
//             private bool isLoading;
//             private bool isActivating;
//             private bool isUnloading;
//
//             public Scene(bool isLoaded, bool isActivated, string name)
//             {
//                 _name = name;
//                 IsLoaded = isLoaded;
//                 IsActivated = isActivated;
//             }
//
//             public async Task LoadAsync()
//             {
//                 if (IsLoaded)
//                 {
//                     throw new Exception(_name+": " + "Cannot load Scene. Scene is already loaded.");
//                 }
//
//                 if (isLoading)
//                 {
//                     throw new Exception(_name+": " + "Cannot load Scene. Already loading.");
//                 }
//
//                 isLoading = true;
//                 Console.WriteLine(_name+": " + "Loading Scene.");
//                 await Task.Delay(600);
//                 Console.WriteLine(_name+": " + "Finished loading Scene.");
//                 isLoading = false;
//                 IsLoaded = true;
//             }
//
//             public async Task ActivateAsync()
//             {
//                 if (!IsLoaded)
//                 {
//                     throw new Exception(_name+": " + "Cannot activate Scene. Scene is not loaded.");
//                 }
//                 if (IsActivated)
//                 {
//                     throw new Exception(_name+": " + "Cannot activate Scene. Scene is already activated.");
//                 }
//
//                 if (isActivating)
//                 {
//                     throw new Exception(_name+": " + "Cannot activate Scene. Already activating.");
//                 }
//
//                 if (!LoadingScreen.Shown)
//                 {
//                     throw new Exception(_name+": " + "Activating a Scene while the loading screen is not shown does not look good on the screen!");
//                 }
//
//                 isActivating = true;
//                 Console.WriteLine(_name+": " + "Activating Scene.");
//                 await Task.Delay(300);
//                 Console.WriteLine(_name+": " + "Finished activating Scene.");
//                 isActivating = false;
//                 IsActivated = true;
//             }
//
//             public async Task UnloadAsync()
//             {
//                 if (!IsLoaded)
//                 {
//                     throw new Exception(_name+": " + "Cannot unload Scene. Scene is not loaded.");
//                 }
//
//                 if (isUnloading)
//                 {
//                     throw new Exception(_name+": " + "Cannot unload Scene. Already unloading.");
//                 }
//
//                 if (!LoadingScreen.Shown && IsActivated)
//                 {
//                     throw new Exception(_name+": " + "Unloading an active Scene while the loading screen is not shown does not look good on the screen!");
//                 }
//
//                 isUnloading = true;
//                 Console.WriteLine(_name+": " + "Unloading Scene.");
//                 await Task.Delay(300);
//                 Console.WriteLine(_name+": " + "Finished unloading Scene.");
//                 isUnloading = false;
//                 IsLoaded = false;
//                 IsActivated = false;
//             }
//
//             public bool IsLoaded { get; private set; }
//             public bool IsActivated { get; private set; }
//         }
//         
//         static void Main()
//         {
//             var oldScene = new Scene(true, true, "oldScene");
//             var newScene = new Scene(false, false, "newScene");
//             var stopwatch = Stopwatch.StartNew();
//             SceneSwitcher.SwitchScenesAsync(oldScene, newScene).Wait();
//             stopwatch.Stop();
//             if (oldScene.IsLoaded)
//             {
//                 throw new Exception("The old scene should have been unloaded by now.");
//             }
//
//             if (!newScene.IsActivated)
//             {
//                 throw new Exception("The new scene should be active by now.");
//             }
//
//             if (LoadingScreen.Shown)
//             {
//                 throw new Exception("The loading screen should be hidden by now.");
//             }
//
//             Console.WriteLine($"Time elapsed: {stopwatch.Elapsed}. Compare this to the results of other students.");
//         }
//     }
// }