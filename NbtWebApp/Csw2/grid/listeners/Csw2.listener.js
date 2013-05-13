(function _listenerIIFE(){

     /**
      * The private constructor for a Listeners object.
     */
      var GridListeners = function () {
          var that = this;
          Object.defineProperties(that, {
              add: {
                  value: function(name, method) {
                      if (!(Csw2.constants.gridListeners.has(name))) {
                          throw new Error('Grid listener type ' + name + ' is not supported.');
                      }
                      var listener = Csw2.grids.listeners[name](method);

                      Object.defineProperty(that, name, {
                          value: listener,
                          writable: true,
                          configurable: true,
                          enumerable: true
                      });
                  }
              }
          });
          return that;
      };

      Csw2.instanceof.lift('GridListeners', GridListeners);

     /**
      * Create a new listeners collection. This returns a listeners object with an add method.
     */
      Csw2.grids.listeners.lift('listeners', function (){
          var ret = new GridListeners();
          return ret;
      });


      }());