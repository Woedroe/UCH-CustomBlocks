using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace CustomBlocks.CustomBlocks
{
    class ToggleBlockA : CustomBlock
    {
        public override int BasedId { get { return 0; } }
        public override string BasePlaceableName { get { return "01_1x1 Box"; } }
        public override string BasePickableBlockName { get { return "01_1x1 Box_Pick"; } }
        public override string Name { get { return GetType().Name; } }
        
        public new static int StaticId { get; set; }
        public override int CustomId
        {
            get { return StaticId; }
            set { StaticId = value; }
        }

        // Array of sprites
        protected Sprite[] sprites = new Sprite[4];
        protected bool spritesLoaded = false;

        new public Sprite sprite
        {
            get
            {
                LoadAllSprites();
                return sprites[0]; // Return normal sprite for initial setup
            }
        }

        private void LoadAllSprites()
        {
            if (!spritesLoaded)  // Use your bool flag instead of checking spNormal
            {
                string basePath = CustomBlock.ImageDir;
                Debug.Log($"[MyCustomBlock] Loading sprites from: {basePath}");
                
                // Load normal sprite (red) - sprites[0]
                string path1 = Path.Combine(basePath, "MyCustomBlock.png");
                if (File.Exists(path1))
                {
                    Texture2D texNormal = LoadTexture(path1);
                    sprites[0] = Sprite.Create(texNormal, new Rect(0, 0, texNormal.width, texNormal.height), new Vector2(0.5f, 0.5f), 100f);
                    Debug.Log("[MyCustomBlock] Loaded MyCustomBlock.png");
                }
                else
                {
                    Debug.LogError($"[MyCustomBlock] NOT FOUND: {path1}");
                }
                
                // Load flash sprite (pink) - sprites[1]
                string path2 = Path.Combine(basePath, "MyCustomBlock-flash.png");
                if (File.Exists(path2))
                {
                    Texture2D texFlash = LoadTexture(path2);
                    sprites[1] = Sprite.Create(texFlash, new Rect(0, 0, texFlash.width, texFlash.height), new Vector2(0.5f, 0.5f), 100f);
                    Debug.Log("[MyCustomBlock] Loaded MyCustomBlock-flash.png");
                }
                else
                {
                    Debug.LogError($"[MyCustomBlock] NOT FOUND: {path2}");
                }
                
                // Load invis sprite (black) - sprites[2]
                string path3 = Path.Combine(basePath, "MyCustomBlock-invis.png");
                if (File.Exists(path3))
                {
                    Texture2D texInvis = LoadTexture(path3);
                    sprites[2] = Sprite.Create(texInvis, new Rect(0, 0, texInvis.width, texInvis.height), new Vector2(0.5f, 0.5f), 100f);
                    Debug.Log("[MyCustomBlock] Loaded MyCustomBlock-invis.png");
                }
                else
                {
                    Debug.LogError($"[MyCustomBlock] NOT FOUND: {path3}");
                }
                
                // Load invis flash sprite (white) - sprites[3]
                string path4 = Path.Combine(basePath, "MyCustomBlock-invis-flash.png");
                if (File.Exists(path4))
                {
                    Texture2D texInvisFlash = LoadTexture(path4);
                    sprites[3] = Sprite.Create(texInvisFlash, new Rect(0, 0, texInvisFlash.width, texInvisFlash.height), new Vector2(0.5f, 0.5f), 100f);
                    Debug.Log("[MyCustomBlock] Loaded MyCustomBlock-invis-flash.png");
                }
                else
                {
                    Debug.LogError($"[MyCustomBlock] NOT FOUND: {path4}");
                }
                
                spritesLoaded = true;  // Mark as loaded
            }
        }

        // Variables for the disappearing behavior
        private float timer = 0f;
        private float interval = 4f;
        private float flashStart = 3f;
        private float flashSpeed = 0.2f;
        private bool isVisible = true;
        
        public Vector3 startPosition;
        public Quaternion startRotation;

        override public PickableBlock CreatePickableBlock()
        {
            PickableBlock pb = base.CreatePickableBlock();
            pb.transform.localPosition -= new Vector3(15.08f, 20.5f, 1);
            pb.transform.localScale = new Vector3(0.75f, 0.75f, 1);
            return pb;
        }

        override public Placeable CreatePlaceablePrefab()
        {
            Placeable placeable = base.CreatePlaceablePrefab();
            placeable.gameObject.AddComponent(GetType());
            return placeable;
        }

        override public void FixSprite(Transform sprite_holder)
        {
            LoadAllSprites();
            if (sprite_holder && sprites[0] != null)
            {
                sprite_holder.GetComponent<SpriteRenderer>().sprite = sprites[0];
                sprite_holder.transform.localScale = new Vector3(2, 2, 1);
                sprite_holder.transform.localPosition = new Vector3(0f, 0f, 0);
            }
        }

        public override void OnPlace(Placeable placeable, int playerNumber, bool sendEvent, bool force = false)
        {
            LoadAllSprites();
            base.OnPlace(placeable, playerNumber, sendEvent, force);
            this.placed = true; // Mark as placed!
            timer = 0f;
            isVisible = true;
            
            startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            startRotation = transform.rotation;
            
            Debug.Log("[MyCustomBlock] Block placed! Starting timer.");
        }

        // Use Update() like FloatyCloud does
        void Update()
        {
            if (placed) // Check if block is placed
            {
                GameControl gameControl = LobbyManager.instance?.CurrentGameController;
                if (gameControl != null)
                {
                    if (gameControl.Phase == GameControl.GamePhase.PLAY || 
                        gameControl.Phase == GameControl.GamePhase.SUDDENDEATH)
                    {
                        timer += Time.deltaTime;

                        SpriteRenderer sr = transform.Find("Sprite").GetComponent<SpriteRenderer>();
                        
                        if (timer >= interval)
                        {
                            // Switch state
                            timer = 0f;
                            isVisible = !isVisible;
                            
                            // Toggle colliders
                            GameObject solidCollider = transform.Find("SolidCollider").gameObject;
                            solidCollider.SetActive(isVisible);
                            
                            Transform innerHazard = transform.Find("InnerHazard");
                            if (innerHazard != null)
                            {
                                innerHazard.gameObject.SetActive(isVisible);
                            }
                            
                            foreach (Collider2D col in GetComponents<Collider2D>())
                            {
                                col.enabled = isVisible;
                            }
                            
                            // Set sprite: 0=normal, 2=invis
                            sr.sprite = sprites[isVisible ? 0 : 2];
                            
                            Debug.Log($"[MyCustomBlock] State: {(isVisible ? "SOLID" : "PASS-THROUGH")}");
                        }
                        else if (timer >= flashStart)
                        {
                            // Flashing phase
                            float flashTime = (timer - flashStart) % flashSpeed;
                            bool showFlash = flashTime < (flashSpeed / 2f);
                            
                            if (isVisible)
                            {
                                // Red -> Pink flash
                                sr.sprite = sprites[showFlash ? 1 : 0];
                            }
                            else
                            {
                                // Black -> White flash
                                sr.sprite = sprites[showFlash ? 3 : 2];
                            }
                        }
                        else
                        {
                            // Normal state
                            sr.sprite = sprites[isVisible ? 0 : 2];
                        }
                    }
                }
            }
        }
    }
}