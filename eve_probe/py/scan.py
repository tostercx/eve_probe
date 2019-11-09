import state

scanning = False,

def step(pck):
    if '__class__' in pck:
        if pck['__class__'].endswith('.Notification'):
            if 'broadcastID' in pck['destination'] and pck['destination']['broadcastID'] == 'OnSignalTrackerFullState':
                state.status['signatures'] = {} # reset list
                tracked = pck['payload'][1]['__content__'][1][1][1][1] # last is type of anomaly
                for key in tracked:
                    item = tracked[key]
                    if item['__class__'].endswith('.SignatureInfo'):
                        state.status['signatures'][item['targetID']] = item
