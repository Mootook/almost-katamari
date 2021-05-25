# Almost Katamari
Prototyping features to match Katamari Damacy

## TODO

- [x] Ball does not rotate over objects, force applied just translates across while it actually should fudge the physics so that the katamari can always be rolled over. This might involve a physics shaders??
    - [Physics Shader Articles](https://medium.com/sun-dog-studios/rapid-unity-tutorials-1-physics-materials-68758351fd8a)
    - Friction on props
- [x] Prefab the prop object and see if there's a way to script a set of colliders for it.
    - [ ] Create a bunch of prefab variants for different props
- [ ] Test on slopes
- [ ] Make things fly off
- [ ] Transparency Shader
- [ ] Polish momentum and force of Katamari movement
- [ ] Show latest item picked up in GUI
    - [ ] Draw segment from the actual item to the GUI?
- [ ] The transparency shader against the wall.
    - [Open Source Implementation](https://github.com/a-chancey/roll_a_ball/tree/master/Assets/shaders)

## Controls

![controls](./img/controls.png)