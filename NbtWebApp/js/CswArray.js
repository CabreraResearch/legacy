///// <reference path="../jquery/jquery-1.5.2-vsdoc.js" />
///// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
///// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
///// <reference path="_Global.js" />

//; (function ($) { /// <param name="$" type="jQuery" />
//    $.fn.CswArray = function (array)
//    {

//        var cswArray = array;
//        
//        cswArray.prototype = new Array();

//        // isEmpty()
//        cswArray.prototype.isEmpty = function ()
//        {
//            return this.length === 0;
//        }

//        // getFirst()
//        cswArray.prototype.getFirst = function ()
//        {
//            if (this.IsEmpty())
//                return null;
//            else
//                return this[0];
//        }

//        // getLast()
//        cswArray.prototype.getLast = function ()
//        {
//            if (this.IsEmpty())
//                return null;
//            else
//                return this[this.length - 1];
//        }

//        // clear()
//        cswArray.prototype.clear = function ()
//        {
//            this.splice(0, this.length);
//        };

//        // sum()
//        cswArray.prototype.sum = function ()
//        {
//            var total = new Number(0);
//            for (var i = 0; i < (this.length - 1); i++)
//            {
//                if (isNumeric(this))
//                {
//                    total += this;
//                }
//            }
//            return total;
//        };

//        // min()
//        cswArray.prototype.min = function ()
//        {
//            var min;
//            for (var i = 0; i < (this.length - 1); i++)
//            {
//                if (isNumeric(this))
//                {
//                    if (isNullOrEmpty(min)) min = this;
//                    if (min > this) min = this;
//                }
//            }
//            return min;
//        };

//        // max()
//        cswArray.prototype.max = function ()
//        {
//            var max;
//            for (var i = 0; i < (this.length - 1); i++)
//            {
//                if (isNumeric(this))
//                {
//                    if (isNullOrEmpty(min)) max = this;
//                    if (max < this) max = this;
//                }
//            }
//            return max;
//        };
//        
//        // For proper chaining support
//        return cswArray;

//    }; // function(options) {
//})(jQuery);


