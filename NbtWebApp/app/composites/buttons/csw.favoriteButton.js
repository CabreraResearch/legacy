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
            div: {}
        };
        var cswPublic = {};

        (function _preCtor() {
            Csw.extend(cswPrivate, options, true);
            cswPrivate.div = cswParent.div({ cssclass: 'cswInline' });
            cswPublic = Csw.dom(cswPrivate.div);
        }());
        
        cswPrivate.onFavorite = function () {
            Csw.ajaxWcf.post({
                urlMethod: 'Nodes/toggleFavorite',
                data: cswPrivate.nodeid,
                success: function (response) {
                    if (response.isFavorite) {
                        cswPrivate.addFavoriteIcon.hide();
                        cswPrivate.removeFavoriteIcon.show();
                        onAddFavoriteSuccess();
                    } else {
                        cswPrivate.removeFavoriteIcon.hide();
                        cswPrivate.addFavoriteIcon.show();
                        onRemoveFavoriteSuccess();
                    }
                }
            });
        };
        
        cswPrivate.addFavoriteIcon = cswPrivate.div.icon({
            name: cswPrivate.name + 'addFavoriteBtn',
            iconType: Csw.enums.iconType.star,
            hovertext: 'Add To Favorites',
            size: cswPrivate.size,
            isButton: true,
            onClick: function () {
                cswPrivate.onFavorite();
            }
        });
        
        cswPrivate.removeFavoriteIcon = cswPrivate.div.icon({
            name: cswPrivate.name + 'removeFavoriteBtn',
            iconType: Csw.enums.iconType.starsolid,
            hovertext: 'Remove from Favorites',
            size: cswPrivate.size,
            isButton: true,
            onClick: function () {
                cswPrivate.onFavorite();
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