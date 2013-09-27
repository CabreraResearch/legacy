module('Csw.boolean');
test('bool - evaluates to true', function () {
    deepEqual(Csw.bool(true), true, 'true should be true');
    deepEqual(Csw.bool('true'), true, '"true" should be true');
    deepEqual(Csw.bool(1), true, '1 should be true');
    deepEqual(Csw.bool('1'), true, '"1" should be true');
    deepEqual(Csw.bool(null, true), true, 'null should be true if explicitly specified as such');
});

test('bool - evaluates to false', function () {
    deepEqual(Csw.bool(false), false, 'false should be false');
    deepEqual(Csw.bool('false'), false, '"false" should be false');
    deepEqual(Csw.bool(0), false, '0 should be false');
    deepEqual(Csw.bool('0'), false, '"0" should be false');
    deepEqual(Csw.bool(null), false, 'null should be false');
    deepEqual(Csw.bool(), false, 'undefined should be false');
});