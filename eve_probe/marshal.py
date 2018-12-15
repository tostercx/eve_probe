import mock
from pprint import pformat
import eveprefs
import builtinmangler
import blue
import traceback


import carbon
import explorationscanner
import utillib
import eve
import eveexceptions
import crimewatch


eveprefs.boot = mock.Mock()
eveprefs.boot.role = 'client'
builtinmangler.MangleBuiltins()


inits = {
    'crimewatch.corp_aggression.settings.AggressionSettings': lambda cls, x: cls(x['_enableAfter'], x['_disableAfter']),
    # fromAttrib often not set, but needed for init
    #'eve.common.script.dogma.effect.BrainEffect': lambda cls, x: cls(x['fromAttrib'], x['toItemID'], x['modifierType'], x['toAttribID'], x['operation'], x['extras'] if x['extras'] else tuple())
}


fakeobjs = [
    # fromAttrib often not set, but needed for init
    'eve.common.script.dogma.effect.BrainEffect',
    
    # 404, cant find this class
    #'explorationscanner.common.cosmicAnomalyInfo.CosmicAnomalyInfo',
    #'explorationscanner.common.structureInfo.StructureInfo'
    #'explorationscanner.common.signatureInfo.SignatureInfo'
]


def serialize(x):
    if x.__class__.__module__ == '__builtin__':
        if hasattr(x, '__iter__'):
            it = list(x) if x.__class__.__name__ == 'tuple' else x
            keys = it.keys() if x.__class__.__name__ == 'dict' else range(len(it))
            for key in keys:
                it[key] = serialize(it[key])
            return tuple(it) if x.__class__.__name__ == 'tuple' else it
        else:
            return x
    else: 
        ret = {'__class__': x.__class__.__module__ + '.' + x.__class__.__name__,}
        if x.__class__.__name__ == 'MarshalStream':
            ret['__value__'] = x.Str()
            try:
                ret['__content__'] = serialize(blue.marshal.Load(x))
                del ret['__value__']
            except:
                pass
        else:
            for attr, value in x.__dict__.iteritems():
                ret[attr] = serialize(value)
        return ret


def deserialize(x):
    if x.__class__.__name__ == 'dict' and '__class__' in x:
        if x['__class__'] == 'blue.MarshalStream':
            if '__content__' in x:
                return blue.marshal.Save(deserialize(x['__content__']))
            else:
                return blue.MarshalStream(x['__value__'])
        else:
            (mdl, cls) = x['__class__'].rsplit('.', 1)
            if mdl == 'carbon.common.script.net.machoNetPacket':
                ret = eval(x['__class__'])(donttypecheck=True)
            elif x['__class__'] in inits:
                ret = inits[x['__class__']](eval(x['__class__']), x)
            elif mdl.startswith('explorationscanner.common.') or x['__class__'] in fakeobjs:
                ret = object.__new__(eval(x['__class__']))
            else:
                ret = eval(x['__class__'])()
            for key, value in x.iteritems():
                if key != '__class__':
                    setattr(ret, key, deserialize(value))
            return ret
    elif x.__class__.__module__ == '__builtin__' and hasattr(x, '__iter__'):
        it = list(x) if x.__class__.__name__ == 'tuple' else x
        keys = it.keys() if x.__class__.__name__ == 'dict' else range(len(it))
        for key in keys:
            it[key] = deserialize(it[key])
        return tuple(it) if x.__class__.__name__ == 'tuple' else it
    return x


def load(buf):
    try:
        obj = blue.marshal.Load(buf)
        return pformat(serialize(obj))
    except:
        return traceback.format_exc()


def save(buf):
    try:
        obj = deserialize(eval(buf))
        return blue.marshal.Save(obj).Str()
    except:
        return traceback.format_exc()

