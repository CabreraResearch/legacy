/*global Csw:true, QUnit:false, module:false, test:false, asyncTest:false, expect:false*/
/*global start:false, stop:false ok:false, equal:false, notEqual:false, deepEqual:false*/
/*global notDeepEqual:false, strictEqual:false, notStrictEqual:false, raises:false, Faker: false*/

var cswObject = Object.create(null);

module('CswObject', {
    setup: function() {
        var somePeople = [];
        while (somePeople.length < 1000) {
            somePeople.push(Faker.Helpers.createCard());
        }
        Object.defineProperties(cswObject, {
            people: {
                value: somePeople     
            }
        });
    }
});
test('Csw.iterate functions as it should.', function () {

    var peopleCount = 0;
    Csw.iterate(cswObject.people, function(person) {
        peopleCount += 1;
    });
    
    deepEqual(peopleCount, 1000, "Csw.iterate found all 1000 people.");
    
});
    
