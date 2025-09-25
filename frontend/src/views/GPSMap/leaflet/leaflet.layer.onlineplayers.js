import { getLocations, executeConsoleCommand } from '~/api/gameServer';
import { i18n } from '~/plugins/i18n';
import { myConfirm } from '~/plugins/sweetalert2';

// onlinePlayer icon
const onlinePlayerIcon = L.icon({
    iconUrl: new URL('~/assets/images/marker-survivor.png', import.meta.url).href,
    iconRetinaUrl: new URL('~/assets/images/marker-survivor-2x.png', import.meta.url).href,
    iconSize: [25, 48],
    iconAnchor: [12, 24],
    popupAnchor: [0, -20],
});

function formatCoord(latlng) {
    if (latlng == false) return '- E / - N';
    else return '' + Math.abs(latlng.lat).toFixed(0) + (latlng.lat >= 0 ? ' E' : ' W') + ' / ' + Math.abs(latlng.lng).toFixed(0) + (latlng.lng >= 0 ? ' N' : ' S');
}

export function getOnlinePlayersLayer(map, mapInfo, viewPlayerInventory) {
    const onlinePlayersMarkerGroup = L.markerClusterGroup({
        maxClusterRadius: function (zoom) {
            return zoom >= mapInfo.maxZoom ? 10 : 50;
        },
    });

    const setOnlinePlayersLocation = function (data) {
        document.getElementById('mapControlOnlinePlayerCount').innerText = data.length;
        onlinePlayersMarkerGroup.clearLayers();

        for (let i = 0, len = data.length; i < len; i++) {
            const location = data[i];
            const playerId = location.playerId;
            const entityName = location.entityName;
            const position = location.position;

            const container = L.DomUtil.create('div');

            const title = L.DomUtil.create('span', null, container);
            title.innerText = `${i18n.global.t('player')}: ${entityName} (${playerId})`;

            L.DomUtil.create('br', null, container);

            const inventoryButton = L.DomUtil.create('a', null, container);
            inventoryButton.innerText = i18n.global.t('components.playerInventoryDialog.header');
            inventoryButton.href = 'javascript:void(0);';
            inventoryButton.title = 'View inventory';
            L.DomEvent.on(inventoryButton, 'click', () => {
                viewPlayerInventory(playerId, entityName);
            });

            let initialPos = [position.x, position.z];
            const marker = L.marker(initialPos, {
                icon: onlinePlayerIcon,
                draggable: true,
            }).bindPopup(container);
            marker.addEventListener('popupopen', (e) => {
                e.popup._closeButton.href = 'javascript:void(0);';
            });

            marker.on('dragend', async (e) => {
                const newPos = e.target.getLatLng();
                const message = i18n.global.t('views.map.dragendTelePlayerConfirm', [entityName, formatCoord(newPos)]);
                if (await myConfirm({ text: message })) {
                    await executeConsoleCommand(`tele ${playerId} ${Math.round(newPos.lat)} -1 ${Math.round(newPos.lng)}`);
                    initialPos = [newPos.lat, newPos.lng];
                } else {
                    marker.setLatLng(initialPos);
                }
            });

            marker.bindTooltip(entityName + '<br>' + playerId, { permanent: false, opacity: 1.0 });

            marker.setOpacity(1.0);

            onlinePlayersMarkerGroup.addLayer(marker);
        }
    };

    const updateOnlinePlayersEvent = async function () {
        const data = await getLocations('onlineplayer');
        setOnlinePlayersLocation(data);
    };

    map.on('overlayadd', function (e) {
        if (e.layer == onlinePlayersMarkerGroup) {
            updateOnlinePlayersEvent();
        }
    });

    return onlinePlayersMarkerGroup;
}
