# Almost Katamari
Prototyping features to match Katamari Damacy

## TODO
- [ ] Make things fly off
- [ ] Show latest item picked up in GUI
    - [ ] Draw segment from the actual item to the GUI?
- [ ] Polish climbing
    - When Katamari's sphere collider is covered in other collider's, the collision logic fails.
    - Currently deciding between fudging velocity.y or applying upwards force
- [ ] Update prop prefabs to modify collider shape and not their parent transform.
- [ ] The transparency shader against the wall.
    - [Open Source Implementation](https://github.com/a-chancey/roll_a_ball/tree/master/Assets/shaders)
- [ ] Bug: If both inputs are forward, rotation input is still processed.

## Controls

![controls](./img/controls.png)


### Links

Basically a Frankenstein of the following:
- [UnderGear's Klonmari](https://github.com/UnderGear/Klonamari)
- [Catlike Coding's Tutorials](https://catlikecoding.com/unity/tutorials/movement/)
