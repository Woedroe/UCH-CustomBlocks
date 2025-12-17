using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace CustomBlocks.CustomBlocks
{
    abstract class ToggleBlockBase : CustomBlock
    {
        // Override these in subclasses
        public abstract bool StartsVisible { get; }
        public abstract string SpriteName { get; }  // e.g. "MyCustomBlock" or "MyCustomBlockB"
        public abstract Vector2 TileSize { get; }
        public abstract Vector3 PickableOffset { get; }
        public abstract Vector3 SpriteScale { get; }

        protected Sprite[] sprites = new Sprite[4];
        protected bool spritesLoaded = false;

        new public Sprite sprite
        {
            get
            {
                LoadAllSprites();
                return sprites[StartsVisible ? 0 : 2];
            }
        }

        private void LoadAllSprites()
        {
            if (!spritesLoaded)
            {
                string basePath = CustomBlock.ImageDir;
                
                string path1 = Path.Combine(basePath, $"{SpriteName}.png");
                if (File.Exists(path1))
                {
                    Texture2D tex = LoadTexture(path1);
                    sprites[0] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                }
                
                string path2 = Path.Combine(basePath, $"{SpriteName}-flash.png");
                if (File.Exists(path2))
                {
                    Texture2D tex = LoadTexture(path2);
                    sprites[1] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                }
                
                string path3 = Path.Combine(basePath, $"{SpriteName}-invis.png");
                if (File.Exists(path3))
                {
                    Texture2D tex = LoadTexture(path3);
                    sprites[2] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                }
                
                string path4 = Path.Combine(basePath, $"{SpriteName}-invis-flash.png");
                if (File.Exists(path4))
                {
                    Texture2D tex = LoadTexture(path4);
                    sprites[3] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
                }
                
                spritesLoaded = true;
            }
        }

        private float timer = 0f;
        private float interval = 4f;
        private float flashStart = 3f;
        private float flashSpeed = 0.2f;
        private bool isVisible;
        private bool hasResetThisRound = false;
        
        public Vector3 startPosition;
        public Quaternion startRotation;

        override public PickableBlock CreatePickableBlock()
        {
            PickableBlock pb = base.CreatePickableBlock();
            pb.transform.localPosition -= PickableOffset;
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
            if (sprite_holder && sprites[StartsVisible ? 0 : 2] != null)
            {
                SpriteRenderer sr = sprite_holder.GetComponent<SpriteRenderer>();
                sr.sprite = sprites[StartsVisible ? 0 : 2];
                sr.drawMode = SpriteDrawMode.Tiled;
                sr.size = TileSize;  // NEW - controls how many times to tile
                sprite_holder.transform.localScale = SpriteScale;
                sprite_holder.transform.localPosition = new Vector3(0f, 0f, 0);
            }
        }

        public override void OnPlace(Placeable placeable, int playerNumber, bool sendEvent, bool force = false)
        {
            LoadAllSprites();
            base.OnPlace(placeable, playerNumber, sendEvent, force);
            this.placed = true;
            timer = 0f;
            isVisible = StartsVisible;
            hasResetThisRound = false;
            
            startPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            startRotation = transform.rotation;
        }

        private void SetBlockState(bool visible)
        {
            isVisible = visible;
            
            GameObject solidCollider = transform.Find("SolidCollider")?.gameObject;
            if (solidCollider != null) solidCollider.SetActive(visible);
            
            Transform innerHazard = transform.Find("InnerHazard");
            if (innerHazard != null) innerHazard.gameObject.SetActive(visible);
            
            foreach (Collider2D col in GetComponents<Collider2D>())
            {
                col.enabled = visible;
            }
            
            SpriteRenderer sr = transform.Find("Sprite")?.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = sprites[visible ? 0 : 2];
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
                        if (!hasResetThisRound)
                        {
                            timer = 0f;
                            SetBlockState(StartsVisible);
                            hasResetThisRound = true;
                        }
                        
                        timer += Time.deltaTime;

                        SpriteRenderer sr = transform.Find("Sprite").GetComponent<SpriteRenderer>();
                        
                        if (timer >= interval)
                        {
                            timer = 0f;
                            SetBlockState(!isVisible);
                        }
                        else if (timer >= flashStart)
                        {
                            float flashTime = (timer - flashStart) % flashSpeed;
                            bool showFlash = flashTime < (flashSpeed / 2f);
                            
                            if (isVisible)
                            {
                                sr.sprite = sprites[showFlash ? 1 : 0];
                            }
                            else
                            {
                                sr.sprite = sprites[showFlash ? 3 : 2];
                            }
                        }
                        else
                        {
                            sr.sprite = sprites[isVisible ? 0 : 2];
                        }
                    }
                    else
                    {
                        hasResetThisRound = false;
                        
                        if (!isVisible)
                        {
                            SetBlockState(true);
                        }
                    }
                }
            }
        }
    }
}