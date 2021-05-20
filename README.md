# CardGame
A .NET Standard framework for building card-centric multiplayer CCGs.

*NOTE: This project is still a work in progress, expect breaking changes!*

### The problem
Most frameworks built to create card games expect game designers to define a very limited set of effects that their cards can have, and the game design process ends up being a mix'n'match of those effects to achieve some diversity in the cards. This makes cards very similar to each other, and limits the impact that each card has in a deck.

### The solution
This framework lets you write code to define custom effects, which should be more or less unique for each card. Each card is made of 2 classes, one that contains the base data of the card and one that can override default behaviours with custom ones.

This is an example from the included `SampleGame` project that you can use to understand how to build your game:
```cs
[PlayableCard(typeof(LizzieInstance))]
public class Lizzie : CreatureCard
{
    public Lizzie()
    {
        ShortName = "Lizzie"; // The unique name that identifies this card
        Name = "Lizzie"; // The pretty name of this card
        Description = "When attacking, deal 1 more damage."; // A brief description of the effects of the card.
        Art = "ART_RESOURCE_HERE";

        ManaCost = 2;
        Attack = 1;
        Health = 2;
    }
}

public class LizzieInstance : CreatureCardInstance
{
    // When attacking, deal 1 more damage
    public override int GetAttackDamage(CreatureCardInstance target, bool isAttacking)
        => isAttacking ? Attack + 1 : Attack;
}
```

Features:
- Card-centric
- Unit tests for basic functionalities already included
- Easy to write your own unit tests to make sure your cards behave exactly as they're meant to
- Dedicated server to prevent cheating
- Sample game with .NET 5 Console-based and web-based (Blazor) clients
- Plug and play assemblies to switch between card sets
- Can be integrated with Unity/Godot or other C# based engines
- Includes built-in features such as Rush or Taunt so you don't have to implement them yourself
- Rich event-based notifications when the state of any part of the game changes

Based on:
- [LiteNetLib](https://github.com/RevenantX/LiteNetLib) for reliable UDP messaging
- [spectre.console](https://github.com/spectreconsole/spectre.console) for colored and organized logs
- [picrew.me](https://picrew.me/image_maker/6883) for card arts of the sample game (drawn by ザズズ)

### Screenshots
Dedicated server live game log

![image](https://user-images.githubusercontent.com/50030666/118831410-0ca05d80-b8c0-11eb-83d4-51ea82452d02.png)

The Blazor client

![image](https://user-images.githubusercontent.com/50030666/118871598-002ffb00-b8e8-11eb-8f2b-d021b356c4e1.png)


### Future steps
These are the things I would like to implement if I happen to have some spare time, in order of importance
- Improve the UI of the Blazor client
- Expand the SampleGame with more cards and licensed artworks
- Support for more card feats, spells and sceneries
- Unity or Godot-based game client
- Database for managing player data
- Server that can host multiple games at the same time
- Matchmaking
- AI for solo play

### License
MIT, Copyright Davide Testoni 2021.
