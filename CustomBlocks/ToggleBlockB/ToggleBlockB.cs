using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace CustomBlocks.CustomBlocks
{
    class ToggleBlockB : CustomBlock
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

        protected Sprite[] sprites = new Sprite[4];
        protected bool spritesLoaded = false;

        new public Sprite sprite
        {
            get
            {
                LoadAllSprites();
                return sprites[2];
            }
        }

        private void LoadAllSprites()
        {
            if (!spritesLoaded)
            {
                string basePath = CustomBlock.ImageDir;
                Debug.Log($"[ToggleBlockB] Loading sprites from: {basePath}");
                
                string path1 = Path.Combine(basePath, "MyCustomBlockB.png");
                if (File.Exists(path1))
                {
                    Texture2D texNormal = LoadTexture(path1);
                    sprites[0] = Sprite.Create(texNormal, new Rect(0, 0, texNormal.width, texNormal.height), new Vector2(0.5f, 0.5f), 100f);
                }
                
                string path2 = Path.Combine(basePath, "MyCustomBlockB-flash.png");
                if (File.Exists(path2))
                {
                    Texture2D texFlash = LoadTexture(path2);
                    sprites[1] = Sprite.Create(texFlash, new Rect(0, 0, texFlash.width, texFlash.height), new Vector2(0.5f, 0.5f), 100f);
                }
                
                string path3 = Path.Combine(basePath, "MyCustomBlockB-invis.png");
                if (File.Exists(path3))
                {
                    Texture2D texInvis = LoadTexture(path3);
                    sprites[2] = Sprite.Create(texInvis, new Rect(0, 0, texInvis.width, texInvis.height), new Vector2(0.5f, 0.5f), 100f);
                }
                
                string path4 = Path.Combine(basePath, "MyCustomBlockB-invis-flash.png");
                if (File.Exists(path4))
                {
                    Texture2D texInvisFlash = LoadTexture(path4);
                    sprites[3] = Sprite.Create(texInvisFlash, new Rect(0, 0, texInvisFlash.width, texInvisFlash.height), new Vector2(0.5f, 0.5f), 100f);
                }
                
                spritesLoaded = true;
            }
        }

        private float timer = 0f;
        private float interval = 4f;
        private float flashStart = 3f;
        private float flashSpeed = 0.2f;
        private bool isVisible = false;
        private bool hasResetThisRound = false;  // NEW
        
        public Vector3 startPosition;
        public Quaternion startRotation;

        override public PickableBlock CreatePickableBlock()
        {
            PickableBlock pb = base.CreatePickableBlock();
            pb.transform.localPosition -= new Vector3(15.08f, 18.5f, 1);
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
            if (sprite_holder && sprites[2] != null)
            {
                sprite_holder.GetComponent<SpriteRenderer>().sprite = sprites[2];
                sprite_holder.transform.localScale = new Vector3(2, 2, 1);
                sprite_holder.transform.localPosition = new Vector3(0f, 0f, 0);
            }
        }

        public override void OnPlace(Placeable placeable, int playerNumber, bool sendEvent, bool force = false)
        {
            LoadAllSprites();
            base.OnPlace(placeable, playerNumber, sendEvent, force);
            this.placed = true;
            timer = 0f;
            isVisible = false;
            hasResetThisRound = false;  // NEW
            
            startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            startRotation = transform.rotation;
            
            Debug.Log("[ToggleBlockB] Block placed! Starting timer.");
        }

        void Update()
        {
            if (placed)
            {
                GameControl gameControl = LobbyManager.instance?.CurrentGameController;
                if (gameControl != null)
                {
                    if (gameControl.Phase == GameControl.GamePhase.PLAY || 
                        gameControl.Phase == GameControl.GamePhase.SUDDENDEATH)
                    {
                        // Reset at start of round
                        if (!hasResetThisRound)
                        {
                            timer = 0f;
                            isVisible = false;
                            
                            GameObject solidCollider = transform.Find("SolidCollider")?.gameObject;
                            if (solidCollider != null) solidCollider.SetActive(false);
                            
                            Transform innerHazard = transform.Find("InnerHazard");
                            if (innerHazard != null) innerHazard.gameObject.SetActive(false);
                            
                            foreach (Collider2D col in GetComponents<Collider2D>())
                            {
                                col.enabled = false;
                            }
                            
                            SpriteRenderer sr = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();
                            if (sr != null) sr.sprite = sprites[2];
                            
                            hasResetThisRound = true;
                            Debug.Log("[ToggleBlockB] Reset for new round!");
                        }
                        
                        timer += Time.deltaTime;

                        SpriteRenderer spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
                        
                        if (timer >= interval)
                        {
                            timer = 0f;
                            isVisible = !isVisible;
                            
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
                            
                            spriteRenderer.sprite = sprites[isVisible ? 0 : 2];
                            
                            Debug.Log($"[ToggleBlockB] State: {(isVisible ? "SOLID" : "PASS-THROUGH")}");
                        }
                        else if (timer >= flashStart)
                        {
                            float flashTime = (timer - flashStart) % flashSpeed;
                            bool showFlash = flashTime < (flashSpeed / 2f);
                            
                            if (isVisible)
                            {
                                spriteRenderer.sprite = sprites[showFlash ? 1 : 0];
                            }
                            else
                            {
                                spriteRenderer.sprite = sprites[showFlash ? 3 : 2];
                            }
                        }
                        else
                        {
                            spriteRenderer.sprite = sprites[isVisible ? 0 : 2];
                        }
                    }
                    else
                    {
                        // Not in PLAY phase, reset flag for next round
                        hasResetThisRound = false;
                        
                        // Reset to visible for build phase (so bombs can destroy it)
                        if (!isVisible)
                        {
                            isVisible = true;
                            
                            GameObject solidCollider = transform.Find("SolidCollider")?.gameObject;
                            if (solidCollider != null) solidCollider.SetActive(true);
                            
                            Transform innerHazard = transform.Find("InnerHazard");
                            if (innerHazard != null) innerHazard.gameObject.SetActive(true);
                            
                            foreach (Collider2D col in GetComponents<Collider2D>())
                            {
                                col.enabled = true;
                            }
                            
                            SpriteRenderer sr = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();
                            if (sr != null) sr.sprite = sprites[0];  // Show solid sprite during build
                        }
                    }
                }
            }
        }
    }
}