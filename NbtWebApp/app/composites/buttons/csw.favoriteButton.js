(function () {
    Csw.composites.register('favoriteButton', function (cswParent, options) {
        'use strict';

        var cswPrivate = {
            name: '',
            size: 16,
            nodeid: '',
            isFavorite: false,
            onAddFavoriteSuccess: function() {},
            onRemoveFavoriteSuccess: function () { },
            onClick: function(isFavorite) {},
            div: {}
        };
        var cswPublic = {};

        (function _preCtor() {
            Csw.extend(cswPrivate, options, true);
            cswPrivate.div = cswParent.div({ cssclass: 'cswInline' });
            cswPublic = Csw.dom(cswPrivate.div);
        }());
        
        cswPrivate.addFavoriteIcon = cswPrivate.div.icon({
            name: cswPrivate.name + 'addFavoriteBtn',
            iconType: Csw.enums.iconType.star,
            hovertext: 'Add To Favorites',
            size: cswPrivate.size,
            isButton: true,
            onClick: function () {
                Csw.ajaxWcf.post({
                    urlMethod: 'Nodes/addToFavorites',
                    data: cswPrivate.nodeid,
                    success: function () {
                        cswPrivate.isFavorite = true;
                        cswPrivate.addFavoriteIcon.hide();
                        cswPrivate.removeFavoriteIcon.show();
                        cswPrivate.onAddFavoriteSuccess();
                        cswPrivate.onClick(cswPrivate.isFavorite);
                    }
                });
            }
        });
        
        cswPrivate.removeFavoriteIcon = cswPrivate.div.icon({
            name: cswPrivate.name + 'removeFavoriteBtn',
            iconType: Csw.enums.iconType.starsolid,
            hovertext: 'Remove from Favorites',
            size: cswPrivate.size,
            isButton: true,
            onClick: function () {
                Csw.ajaxWcf.post({
                    urlMethod: 'Nodes/removeFromFavorites',
                    data: cswPrivate.nodeid,
                    success: function () {
                        cswPrivate.isFavorite = false;
                        cswPrivate.removeFavoriteIcon.hide();
                        cswPrivate.addFavoriteIcon.show();
                        cswPrivate.onRemoveFavoriteSuccess();
                        cswPrivate.onClick(cswPrivate.isFavorite);
                    }
                });
            }
        });

        (function _postCtor() {
            if (cswPrivate.isFavorite) {
                cswPrivate.addFavoriteIcon.hide();
            } else {
                cswPrivate.removeFavoriteIcon.hide();
            }
        }());

        return cswPublic;
    });
}());