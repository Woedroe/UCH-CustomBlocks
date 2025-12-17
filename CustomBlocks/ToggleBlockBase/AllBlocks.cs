using UnityEngine;

namespace CustomBlocks.CustomBlocks
{
    // === TYPE A (starts visible) ===
    
    class ToggleBlockA_1x1 : ToggleBlockBase
    {
        public override int BasedId { get { return 0; } }
        public override string BasePlaceableName { get { return "01_1x1 Box"; } }
        public override string BasePickableBlockName { get { return "01_1x1 Box_Pick"; } }
        public override string Name { get { return GetType().Name; } }
        
        public new static int StaticId { get; set; }
        public override int CustomId { get { return StaticId; } set { StaticId = value; } }

        public override bool StartsVisible { get { return true; } }
        public override string SpriteName { get { return "MyCustomBlock"; } }
        public override Vector3 PickableOffset { get { return new Vector3(15.08f, 23.5f, 1); } }
        public override Vector3 SpriteScale { get { return new Vector3(1, 1, 1); } }
        public override Vector2 TileSize { get { return new Vector2(1, 1); } }
    }
    
    class ToggleBlockA_1x3 : ToggleBlockBase
    {
        public override int BasedId { get { return 0; } }
        public override string BasePlaceableName { get { return "03_Plank3"; } }
        public override string BasePickableBlockName { get { return "03_Plank3_Pick"; } }
        public override string Name { get { return GetType().Name; } }
        
        public new static int StaticId { get; set; }
        public override int CustomId { get { return StaticId; } set { StaticId = value; } }

        public override bool StartsVisible { get { return true; } }
        public override string SpriteName { get { return "MyCustomBlock"; } }
        public override Vector3 PickableOffset { get { return new Vector3(15.08f, 24.5f, 1); } }
        public override Vector3 SpriteScale { get { return new Vector3(1, 1, 1); } }
        public override Vector2 TileSize { get { return new Vector2(3, 1); } }
    }
    
    class ToggleBlockA_1x5 : ToggleBlockBase
    {
        public override int BasedId { get { return 0; } }
        public override string BasePlaceableName { get { return "05_Plank5"; } }
        public override string BasePickableBlockName { get { return "05_Plank5_Pick"; } }
        public override string Name { get { return GetType().Name; } }
        
        public new static int StaticId { get; set; }
        public override int CustomId { get { return StaticId; } set { StaticId = value; } }

        public override bool StartsVisible { get { return true; } }
        public override string SpriteName { get { return "MyCustomBlock"; } }
        public override Vector3 PickableOffset { get { return new Vector3(15.08f, 25.5f, 1); } }
        public override Vector3 SpriteScale { get { return new Vector3(1, 1, 1); } }
        public override Vector2 TileSize { get { return new Vector2(5, 1); } }
    }

    // === TYPE B (starts invisible) ===
    
    class ToggleBlockB_1x1 : ToggleBlockBase
    {
        public override int BasedId { get { return 0; } }
        public override string BasePlaceableName { get { return "01_1x1 Box"; } }
        public override string BasePickableBlockName { get { return "01_1x1 Box_Pick"; } }
        public override string Name { get { return GetType().Name; } }
        
        public new static int StaticId { get; set; }
        public override int CustomId { get { return StaticId; } set { StaticId = value; } }

        public override bool StartsVisible { get { return false; } }
        public override string SpriteName { get { return "MyCustomBlockB"; } }
        public override Vector3 PickableOffset { get { return new Vector3(15.08f, 26.5f, 1); } }
        public override Vector3 SpriteScale { get { return new Vector3(1, 1, 1); } }
        public override Vector2 TileSize { get { return new Vector2(1, 1); } }
    }
    
    class ToggleBlockB_1x3 : ToggleBlockBase
    {
        public override int BasedId { get { return 0; } }
        public override string BasePlaceableName { get { return "03_Plank3"; } }
        public override string BasePickableBlockName { get { return "03_Plank3_Pick"; } }
        public override string Name { get { return GetType().Name; } }
        
        public new static int StaticId { get; set; }
        public override int CustomId { get { return StaticId; } set { StaticId = value; } }

        public override bool StartsVisible { get { return false; } }
        public override string SpriteName { get { return "MyCustomBlockB"; } }
        public override Vector3 PickableOffset { get { return new Vector3(15.08f, 27.5f, 1); } }
        public override Vector3 SpriteScale { get { return new Vector3(1, 1, 1); } }
        public override Vector2 TileSize { get { return new Vector2(3, 1); } }
    }

    class ToggleBlockB_1x5 : ToggleBlockBase
    {
        public override int BasedId { get { return 0; } }
        public override string BasePlaceableName { get { return "05_Plank5"; } }
        public override string BasePickableBlockName { get { return "05_Plank5_Pick"; } }
        public override string Name { get { return GetType().Name; } }
        
        public new static int StaticId { get; set; }
        public override int CustomId { get { return StaticId; } set { StaticId = value; } }

        public override bool StartsVisible { get { return false; } }
        public override string SpriteName { get { return "MyCustomBlockB"; } }
        public override Vector3 PickableOffset { get { return new Vector3(15.08f, 28.5f, 1); } }
        public override Vector3 SpriteScale { get { return new Vector3(1, 1, 1); } }
        public override Vector2 TileSize { get { return new Vector2(5, 1); } }
    }
}