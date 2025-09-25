import { useUserInfoStore } from '~/store/userInfo';

export function getSdtdTileLayer(mapInfo, initTime, isMiniMap = false) {
    const token = useUserInfoStore().accessToken;
    const url = import.meta.env.VITE_API_BASE_URL + `GameServer/MapTile/{z}/{x}/{y}?access_token=${token}&t={time}`;
    const tileLayer = L.tileLayer(url, {
        maxZoom: isMiniMap ? mapInfo.maxZoom : mapInfo.maxZoom + 3,
        minZoom: isMiniMap ? -1 : Math.max(0, mapInfo.maxZoom - 5),
        maxNativeZoom: mapInfo.maxZoom,
        minNativeZoom: 0,
        tileSize: mapInfo.tileSize,
        time: initTime,
        attribution: '&copy; <a href="https://7dtd.illy.bz/wiki/Server%20fixes" target="_blank">Alloc Mod</a> contributors',
    });

    tileLayer.getTileUrl = function (coords) {
        coords.y = -coords.y - 1;
        return L.TileLayer.prototype.getTileUrl.bind(tileLayer)(coords);
    };

    return tileLayer;
}
