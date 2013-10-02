var parentDiv;
module('Csw.div', {
    setup: function () {
        parentDiv = $("#qunit-fixture");//This div gets cleaned up after every test run
    }
});
test('div - div is not null', function () {
    var newDiv = Csw.literals.div({ $parent: parentDiv, text: 'test' });
    notDeepEqual(newDiv, null, 'newDiv has been successfully created');
    deepEqual(newDiv.text(), 'test', 'newDiv contains text "test"');
});