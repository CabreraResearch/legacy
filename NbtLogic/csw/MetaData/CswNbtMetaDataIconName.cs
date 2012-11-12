
using System;
using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Nbt Icon File Names
    /// </summary>
    public sealed class NbtIcon : IEquatable<NbtIcon>
    {
        #region Internals
        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
        {
            { DNA, DNA },
            { about, about },
            { add, add },
            { atom, atom },
            { back, back },
            { barchart, barchart },
            { barcode, barcode },
            { bell, bell },
            { box, box },
            { browseropen, browseropen },
            { cabinet, cabinet },
            { calculator, calculator },
            { calendar, calendar },
            { camera, camera },
            { cancel, cancel },
            { cart, cart },
            { cd, cd },
            { check, check },
            { clipboardcheck, clipboardcheck },
            { clipboardgear, clipboardgear },
            { clock, clock },
            { compass, compass },
            { contact, contact },
            { copy, copy },
            { doc, doc },
            { docexport, docexport },
            { docrefresh, docrefresh },
            { door, door },
            { down, down },
            { envelope, envelope },
            { explode, explode },
            { fastforward, fastforward },
            { fax, fax },
            { flag, flag },
            { flask, flask },
            { folder, folder },
            { folder2, folder2 },
            { folder2open, folder2open },
            { foldergear, foldergear },
            { games, games },
            { gear, gear },
            { gearset, gearset },
            { harddrive, harddrive },
            { house, house },
            { image, image },
            { import, import },
            { info, info },
            { largeicons, largeicons },
            { ListText, ListText },
            { Lock, Lock },
            { magglass, magglass },
            { magminus, magminus },
            { magplus, magplus },
            { minus, minus },
            { music, music },
            { network, network },
            { notepad, notepad },
            { options, options },
            { paint, paint },
            { paste, paste },
            { pause, pause },
            { pencil, pencil },
            { people, people },
            { person, person },
            { phone, phone },
            { play, play },
            { print, print },
            { printpreview, printpreview },
            { questionmark, questionmark },
            { redo, redo },
            { refresh, refresh },
            { rewind, rewind },
            { right, right },
            { rightbutton, rightbutton },
            { save, save },
            { saveas, saveas },
            { scissors, scissors },
            { search, search },
            { skipback, skipback },
            { skipforward, skipforward },
            { smallicons, smallicons },
            { square, square },
            { star, star },
            { staradd, staradd },
            { stop, stop },
            { target, target },
            { targetgroup, targetgroup },
            { toolbox, toolbox },
            { trash, trash },
            { trianglegear, trianglegear },
            { undo, undo },
            { unlock, unlock },
            { up, up },
            { wizard, wizard },
            { world, world },
            { wrench, wrench },
            { x, x },
            { xbutton, xbutton }
        };
        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        private static string _Parse( string Val )
        {
            string ret = CswResources.UnknownEnum;
            if( _Enums.ContainsKey( Val ) )
            {
                ret = _Enums[Val];
            }
            return ret;
        }

        /// <summary>
        /// The enum constructor
        /// </summary>
        public NbtIcon( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator NbtIcon( string Val )
        {
            return new NbtIcon( Val );
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public static implicit operator string( NbtIcon item )
        {
            return item.Value;
        }

        /// <summary>
        /// Override of ToString
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        #endregion Internals

        #region Enum members

        /// <summary>
        /// Enum member 1
        /// </summary>
        public const string DNA = "DNA.png";
        public const string about = "about.png";
        public const string add = "add.png";
        public const string atom = "atom.png";
        public const string back = "back.png";
        public const string barchart = "barchart.png";
        public const string barcode = "barcode.png";
        public const string bell = "bell.png";
        public const string box = "box.png";
        public const string browseropen = "browseropen.png";
        public const string cabinet = "cabinet.png";
        public const string calculator = "calculator.png";
        public const string calendar = "calendar.png";
        public const string camera = "camera.png";
        public const string cancel = "cancel.png";
        public const string cart = "cart.png";
        public const string cd = "cd.png";
        public const string check = "check.png";
        public const string clipboardcheck = "clipboardcheck.png";
        public const string clipboardgear = "clipboardgear.png";
        public const string clock = "clock.png";
        public const string compass = "compass.png";
        public const string contact = "contact.png";
        public const string copy = "copy.png";
        public const string doc = "doc.png";
        public const string docexport = "docexport.png";
        public const string docrefresh = "docrefresh.png";
        public const string door = "door.png";
        public const string down = "down.png";
        public const string envelope = "envelope.png";
        public const string explode = "explode.png";
        public const string fastforward = "fastforward.png";
        public const string fax = "fax.png";
        public const string flag = "flag.png";
        public const string flask = "flask.png";
        public const string folder = "folder.png";
        public const string folder2 = "folder2.png";
        public const string folder2open = "folder2open.png";
        public const string foldergear = "foldergear.png";
        public const string games = "games.png";
        public const string gear = "gear.png";
        public const string gearset = "gearset.png";
        public const string harddrive = "harddrive.png";
        public const string house = "house.png";
        public const string image = "image.png";
        public const string import = "import.png";
        public const string info = "info.png";
        public const string largeicons = "largeicons.png";
        public const string ListText = "list.text";
        public const string Lock = "lock.png";
        public const string magglass = "magglass.png";
        public const string magminus = "magminus.png";
        public const string magplus = "magplus.png";
        public const string minus = "minus.png";
        public const string music = "music.png";
        public const string network = "network.png";
        public const string notepad = "notepad.png";
        public const string options = "options.png";
        public const string paint = "paint.png";
        public const string paste = "paste.png";
        public const string pause = "pause.png";
        public const string pencil = "pencil.png";
        public const string people = "people.png";
        public const string person = "person.png";
        public const string phone = "phone.png";
        public const string play = "play.png";
        public const string print = "print.png";
        public const string printpreview = "printpreview.png";
        public const string questionmark = "questionmark.png";
        public const string redo = "redo.png";
        public const string refresh = "refresh.png";
        public const string rewind = "rewind.png";
        public const string right = "right.png";
        public const string rightbutton = "rightbutton.png";
        public const string save = "save.png";
        public const string saveas = "saveas.png";
        public const string scissors = "scissors.png";
        public const string search = "search.png";
        public const string skipback = "skipback.png";
        public const string skipforward = "skipforward.png";
        public const string smallicons = "smallicons.png";
        public const string square = "square.png";
        public const string star = "star.png";
        public const string staradd = "staradd.png";
        public const string stop = "stop.png";
        public const string target = "target.png";
        public const string targetgroup = "targetgroup.png";
        public const string toolbox = "toolbox.png";
        public const string trash = "trash.png";
        public const string trianglegear = "trianglegear.png";
        public const string undo = "undo.png";
        public const string unlock = "unlock.png";
        public const string up = "up.png";
        public const string wizard = "wizard.png";
        public const string world = "world.png";
        public const string wrench = "wrench.png";
        public const string x = "x.png";
        public const string xbutton = "xbutton.png";

        #endregion Enum members

        #region IEquatable (NbtIcon)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( NbtIcon ft1, NbtIcon ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( NbtIcon ft1, NbtIcon ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is NbtIcon ) )
            {
                return false;
            }
            return this == (NbtIcon) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( NbtIcon obj )
        {
            return this == obj;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        public override int GetHashCode()
        {
            int ret = 23, prime = 37;
            ret = ( ret * prime ) + Value.GetHashCode();
            ret = ( ret * prime ) + _Enums.GetHashCode();
            return ret;
        }

        #endregion IEquatable (NbtIcon)

    };
}
