import state
import blue
from time import time

char_orders = []
last = None

g_station = 60003760 # jita
g_solarsys = 30000142 # jita
g_nodeID = 1000000130

g_updateOrderID = 10122
g_charOrderID = 10123
g_itemOrderID = 10124
g_itemOrderRange = 100


def getOrders(type, callID):
    return {'__class__': 'carbon.common.script.net.machoNetPacket.CallReq',
        'applicationID': None,
        'command': 6,
        'compressedPart': 0,
        'contextKey': None,
        'destination': {'__class__': 'carbon.common.script.net.machoNetAddress.MachoAddress',
                     'addressType': 1,
                     'callID': None,
                     'nodeID': g_nodeID,
                     'service': 'marketProxy'},
        'languageID': None,
        'oob': {},
        'payload': (0,
                 {'__class__': 'blue.MarshalStream',
                  '__content__': (1,
                                  'GetOrders',
                                  (type,),
                                  {'machoVersion': 1})}),
        'source': {'__class__': 'carbon.common.script.net.machoNetAddress.MachoAddress',
                'addressType': 2,
                'callID': callID,
                'clientID': 0,
                'service': None},
        'userID': state.status['userID']}


def getCharOrders(callID):
    return {'__class__': 'carbon.common.script.net.machoNetPacket.CallReq',
        'applicationID': None,
        'command': 6,
        'compressedPart': 0,
        'contextKey': None,
        'destination': {'__class__': 'carbon.common.script.net.machoNetAddress.MachoAddress',
                        'addressType': 1,
                        'callID': None,
                        'nodeID': g_nodeID,
                        'service': 'marketProxy'},
        'languageID': None,
        'oob': {},
        'payload': (0,
                    {'__class__': 'blue.MarshalStream',
                    '__content__': (1, 'GetCharOrders', (), {'machoVersion': 1})}),
        'source': {'__class__': 'carbon.common.script.net.machoNetAddress.MachoAddress',
                'addressType': 2,
                'callID': callID,
                'clientID': 0,
                'service': None},
        'userID': state.status['userID']}


def updateOrder(callID, orderID, price, bid, oldPrice, typeID, quantity):
    return {'__class__': 'carbon.common.script.net.machoNetPacket.CallReq',
        'applicationID': None,
        'command': 6,
        'compressedPart': 0,
        'contextKey': None,
        'destination': {'__class__': 'carbon.common.script.net.machoNetAddress.MachoAddress',
                        'addressType': 1,
                        'callID': None,
                        'nodeID': g_nodeID,
                        'service': 'marketProxy'},
        'languageID': None,
        'oob': {},
        'payload': (0,
                    {'__class__': 'blue.MarshalStream',
                    '__content__': (1,
                                    'ModifyCharOrder',
                                    (orderID,
                                    price,
                                    bid,
                                    g_station,
                                    g_solarsys,
                                    oldPrice,
                                    -1 if bid else typeID,
                                    quantity,
                                    blue.os.GetWallclockTimeNow() - 3001436890), # needs proper time sync ...
                                    {'machoVersion': 1})}),
        'source': {'__class__': 'carbon.common.script.net.machoNetAddress.MachoAddress',
                'addressType': 2,
                'callID': callID,
                'clientID': 0,
                'service': None},
        'userID': state.status['userID']}


def step(pck):
    global last, char_orders
    
    if '__class__' in pck and pck['__class__'].endswith('.CallRsp'):
        
        callID = pck.get('destination', {}).get('callID', 0)
        service = pck.get('source', {}).get('service', '')
        
        if service == 'marketProxy' and callID:

            # char orders
            if callID == g_charOrderID:
                try:
                    o = pck['payload']['__content__']['result']['__content__']
                    if o['__class__'] == 'carbon.common.script.sys.crowset.CRowset':

                        char_orders = []
                        order_types = set()
                        state.status['order_updates'] = []

                        for k in o:
                            if k.__class__.__name__ == 'int':
                                char_orders.append(o[k]['orderID'])
                                order_types.add(o[k]['typeID'])

                        state.status['char_orders'] = char_orders
                        i = 0

                        for t in order_types:
                            state.injectQueue.append(getOrders(t, g_itemOrderID + i))
                            i+=1
                except:
                    pass


            # region order lists
            if callID >= g_itemOrderID and callID <= g_itemOrderID + g_itemOrderRange:
                try:
                    o = pck['payload']['__content__']['result']['__content__']
                    if o[0]['__class__'] == 'eve.common.script.sys.rowset.RowList':

                        # sell
                        my_orders = []
                        my_price = 0
                        price = 0
                        for k in o[0]:
                            if k.__class__.__name__ == 'int':

                                co = o[0][k]
                                if co['stationID'] == g_station and co['bid'] == False:

                                    if co['orderID'] in char_orders:
                                        my_orders.append(co)
                                        if not my_price or co['price'] < my_price:
                                            my_price = co['price']

                                    elif not price or co['price'] < price:
                                        price = co['price']

                        if len(my_orders) and price and my_price and price < my_price:
                            for my_order in my_orders:
                                state.status['order_updates'].append((my_order['orderID'], round(price - 0.01, 2)))
                                state.injectQueue.append(updateOrder(g_updateOrderID, my_order['orderID'], round(price - 0.01, 2), False, my_order['price'], my_order['typeID'], my_order['volRemaining']))
                        

                        # buy
                        my_orders = []
                        my_price = 0
                        price = 0
                        for k in o[1]:
                            if k.__class__.__name__ == 'int':

                                co = o[1][k]
                                if co['stationID'] == g_station and co['bid'] == True:

                                    if co['orderID'] in char_orders:
                                        my_orders.append(co)
                                        if co['price'] > my_price:
                                            my_price = co['price']
                                            
                                    elif co['price'] > price:
                                        price = co['price']

                        if len(my_orders) and price and my_price and price > my_price:
                            for my_order in my_orders:
                                state.status['order_updates'].append((my_order['orderID'], round(price + 0.01, 2)))
                                state.injectQueue.append(updateOrder(g_updateOrderID, my_order['orderID'], round(price + 0.01, 2), True, my_order['price'], -1, my_order['volRemaining']))
                
                except Exception as e:
                    pass
    
    # refresh every 5 minutes
    if not last or time() - last > 62 * 5:
        try:
            state.injectQueue.append(getCharOrders(g_charOrderID))
        except:
            pass
        last = time()
