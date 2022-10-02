![Altimit logo](/logo.png)

<i>Note: I'm in the process of open-sourcing this project. This repository will evolve significantly over the next few weeks.</i>

## Altimit

Altimit is a flexible set of tools and protocols for building an open Metaverse. You can use Altimit as an engine for creating your own desktop/AR/VR application, or as a plugin for creating a Metaverse application in your favorite game engine or IDE. Altimit allows your application to seamlessly connect with other applications to create a cohesive experience.

Features of Altimit include:

• <b>Automatic Replication</b>: Describe the data you want replicated using basic attributes or a fluent API. Altimit handles the rest.
```C#
[AType]
public class User
{
  [AProperty]
  public string FirstName { get; set; }
  [AProperty]
  public string LastName { get; set; }
  [AProperty]
  public string Email { get; set; }
  [AProperty]
  public string Password { get; set; }
}
```
• <b>Serialization</b>: Data is automatically cached locally and remotely.

• <b>Seamless RPC</b>: Calling methods on remote classes is as intuitive as calling methods on local ones.
```C#
// The interface of a server
[AType]
public interface IUserServer
{
    [AMethod]
    Task<User> SignIn(string email, string password);
    [AMethod]
    Task Logout();
}

// On client:
var myUser = await UserServer.SignIn(myEmail, myPassword);

// On server:
public async Task<User> SignIn(string email, string password)
{
  ... // Return a user based on the provided email and password
  return user;
}
```
• <b>Distributed Computing</b>: Built-in mesh networking enables large-scale simulations.

• <b>Animation</b>: Create animations and record network interactions for future playback.

• <b>User Interfaces</b>: Easily create complex, scalable, platform-agnostic user interfaces.
```C#
var signInView = new Canvas().Hold(
    new TextInput() { Placeholder = "Email" },
    new TextInput() { Placeholder = "Password", InputType = InputType.Password },
    new Button() { Label = "Sign In" }
);
```
• <b>Voice and Video</b>: Altimit uses WebRTC to enable peer to peer connections, including voice and video.

<i>As of now, C# is the only supported language.</i>

In Altimit, every interaction boils down to <b>data and data changes</b>. This basic notion is integral to Altimit's elasticity—enabling it to play nice with a plethora of third-party editors and frameworks.

## Opening the Metaverse

<i>The logical symbol ∀ is used to represent universal quantification in predicate logic, where it is typically read as "for all".</i>

Unlike [other](https://docs.omniverse.nvidia.com/prod_kit/common/NVIDIA_Omniverse_License_Agreement.html) Metaverse frameworks, Altimit is entirely free and open source under the MIT License. Anyone is welcome to use and contribute to the engine. The goal of this project—if there is one—is to create a super accessible and useful Metaverse framework for developers and end-users. Let's make the future connected, and let's make it beautiful.

## Compatibility / Plugins

Altimit is available as a plugin for Godot, Unity, native Windows and native Linux. Support is planned for native macOS and native iOS in the near future. If you'd like to see support for another platform or language, please consider contributing to the project.

Platform | Support |
--- | --- | 
Godot | Supported ✔️ |
Unity | Supported ✔️ |
Native Windows | Supported ✔️ |
Native Linux | Supported ✔️ |
Native macOS | Planned ➡️ |
Native iOS | Planned ➡️ |
Native Android | Pending  |
Unreal | Pending |
Blender | Pending |

Language | Support |
--- | --- | 
C# | Supported ✔️ |
Swift | Planned ➡️ |
C++ | Pending |
Javascript | Pending |

## The Engine

Altimit's Engine is an alternative to its plugins. Built on top of the Godot Engine, the Altimit Engine combines Godot's cross-platform support and open architecture with Altimit's flexibility. It's a fork of the main branch of the Godot Engine, with a few additional features and optimizations to provide the best possible experience of Altimit.

<i>Note: Whenever possible, developers are encouraged to contribute to the main branch of Godot, rather than contributing directly to Altimit's fork of the Godot Engine. This helps limit fragmentation between projects.</i>
