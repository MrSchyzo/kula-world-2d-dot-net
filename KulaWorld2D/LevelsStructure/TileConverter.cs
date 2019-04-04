using GameUtils;
using System;
using System.Linq;

namespace LevelsStructure
{
    /// <summary>
    /// Classe statica utilizzata per convertire tipi specifici di tile da stringhe a byte e viceversa.
    /// </summary>
    public static class TileConverter
    {
        /// <summary>
        /// Array contenente i SurfaceType possibili.
        /// </summary>
        
        public static readonly KulaLevel.SurfaceType[] surfaces =
            new KulaLevel.SurfaceType[9]
            {
                KulaLevel.SurfaceType.Nothing,
                KulaLevel.SurfaceType.Ice,
                KulaLevel.SurfaceType.Fire,
                KulaLevel.SurfaceType.Spikes,
                KulaLevel.SurfaceType.TimedSpikes,
                KulaLevel.SurfaceType.Ramp,
                KulaLevel.SurfaceType.Teleport,
                KulaLevel.SurfaceType.NoJump,
                KulaLevel.SurfaceType.Exit
            };
        

        /// <summary>
        /// Array contenente i tipi di blocchi possibili.
        /// </summary>
        
        public static readonly string[] blocks =
            new string[6]
            {
                "Normal",
                "Transparent",
                "Intermittent",
                "Ice",
                "Fire",
                "Destructible"
            };
        

        /// <summary>
        /// Array contenente i tipi di oggetti posizionabili possibili.
        /// </summary>
        
        public static readonly string[] placeables =
            new string[14]
            {
                "Bronze Coin",
                "Silver Coin",
                "Golden Coin",
                "Sapphire",
                "Ruby",
                "Emerald",
                "Diamond",
                "Key",
                "Fruit",
                "Glasshour",
                "Slow Pill",
                "Glasses",
                "Gravity Changer",
                "Spawn Point"
            };
        

        /// <summary>
        /// Array contenente i tipi di nemici possibili.
        /// </summary>
        
        public static readonly string[] enemies =
            new string[2]
            {
                "SinusoidalEnemy",
                "Jumper"
            };
        

        /// <summary>
        /// Restituisce l'array contenente i tipi specifici di un dato tipo di tile, sottoforma di stringa
        /// </summary>
        /// <param name="tt">Tipo di tile di cui si vuole ottenere le stringhe relative ai tipi specifici.</param>
        /// <returns></returns>
        public static string[] GetSpecificTypesOf(KulaLevel.TileType tt)
        {
            if (tt == KulaLevel.TileType.Block)
                return blocks.DeepClone<string[]>();
            else if (tt == KulaLevel.TileType.Enemy)
                return enemies.DeepClone<string[]>();
            else if (tt == KulaLevel.TileType.Placeable)
                return placeables.DeepClone<string[]>();
            else
                return null;
        }

        //Metodi statici di conversione dei tipi: FromStringSpecificType, FromByteSpecificByte, StringToSurfaceType, SurfaceTypeToString
        
        /// <summary>
        /// Restituisce la stringa relativa al tipo specifico di tile indicato in input. Viene restituita la stringa vuota
        /// in caso di mancata conversione.
        /// </summary>
        /// <param name="tt">Tipo di tile di cui si vuole sapere il tipo specifico.</param>
        /// <param name="t">Tipo specifico della tile.</param>
        /// <returns>Tipo specifico della tile convertito in stringa.</returns>
        public static string FromByteSpecificType(KulaLevel.TileType tt, byte t)
        {
            string res = "";
            if (tt == KulaLevel.TileType.Block)
            {
                if (t >= 0 && t < blocks.Length)
                    res = blocks[t];
            }
            else if (tt == KulaLevel.TileType.Placeable)
            {
                if (t >= 0 && t < placeables.Length)
                    res = placeables[t];
            }
            else if (tt == KulaLevel.TileType.Enemy)
            {
                if (t >= 0 && t < enemies.Length)
                    res = enemies[t];
            }
            else
                throw new Exception("The input type is unknown: " + tt.ToString());
            return res;
        }

        /// <summary>
        /// Restituisce il tipo specifico di un tile sottoforma di byte. In caso di stringa non valida, viene restituito 255
        /// </summary>
        /// <param name="tt">Tipo di tile in input</param>
        /// <param name="s">Stringa che dovrebbe rappresentare un tipo specifico di tile.</param>
        /// <returns></returns>
        public static byte FromStringSpecificType(KulaLevel.TileType tt, string s)
        {
            byte res = 255;
            if (tt == KulaLevel.TileType.Block)
                res = (byte)(blocks.ToList<string>().FindIndex(0, (x => x == s)));
            else if (tt == KulaLevel.TileType.Placeable)
                res = (byte)(placeables.ToList<string>().FindIndex(0, (x => x == s)));
            else if (tt == KulaLevel.TileType.Enemy)
                res = (byte)(enemies.ToList<string>().FindIndex(0, (x => x == s)));
            return res;
        }

        /// <summary>
        /// Restituisce il tipo di superficie a partire dalla stringa data.
        /// </summary>
        /// <param name="s">Stringa data.</param>
        /// <returns></returns>
        public static KulaLevel.SurfaceType StringToSurfaceType(string s)
        {
            if (surfaces.ToList<KulaLevel.SurfaceType>().Exists(x => x.ToString().Equals(s)))
                return surfaces.ToList<KulaLevel.SurfaceType>().First<KulaLevel.SurfaceType>(x => x.ToString().Equals(s));
            else
                return KulaLevel.SurfaceType.Nothing;
        }

        /// <summary>
        /// Restituisce la stringa associata il tipo di superficie specificata
        /// </summary>
        /// <param name="st">Tipo di superficie in input</param>
        /// <returns></returns>
        public static string SurfaceTypeToString(KulaLevel.SurfaceType st)
        {
            return st.ToString();
        }

        /// <summary>
        /// Restituisce l'orientamento a partire da una stringa data, restituisce Down in caso di mismatch
        /// </summary>
        /// <param name="s">Stringa che indica l'orientamento desiderato</param>
        /// <returns></returns>
        public static KulaLevel.Orientation StringToOrientation(string s)
        {
            switch (s)
            {
                case ("Up"):
                    return KulaLevel.Orientation.Up;
                case ("Down"):
                    return KulaLevel.Orientation.Down;
                case ("Left"):
                    return KulaLevel.Orientation.Left;
                case ("Right"):
                    return KulaLevel.Orientation.Right;
                default:
                    return KulaLevel.Orientation.Down;
            }
        }

        /// <summary>
        /// Restituisce l'orientamento inversa a quella data.
        /// </summary>
        /// <param name="o">Orientamento di cui si vuol sapere l'inverso.</param>
        /// <returns></returns>
        public static KulaLevel.Orientation Reverse(KulaLevel.Orientation o)
        {
            if (o == KulaLevel.Orientation.Up)
                return KulaLevel.Orientation.Down;
            else if (o == KulaLevel.Orientation.Down)
                return KulaLevel.Orientation.Up;
            else if (o == KulaLevel.Orientation.Left)
                return KulaLevel.Orientation.Right;
            else if (o == KulaLevel.Orientation.Right)
                return KulaLevel.Orientation.Left;
            else
                return KulaLevel.Orientation.Up;
        }
        
    }
    
}
