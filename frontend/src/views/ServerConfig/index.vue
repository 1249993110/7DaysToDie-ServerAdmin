<template>
    <Card>
        <template #content>
            <Fieldset v-for="(item, index) in modelValue" :key="index" :legend="item.group" toggleable>
                <DataTable :value="item.children" dataKey="name" size="small" :pt="{ headerRow: { class: '!hidden' } }">
                    <Column prop="name" class="w-80">
                        <template #body="{ data }">
                            <Tag class="font-bold font-mono">{{ getName(data.name) }}</Tag>
                        </template>
                    </Column>
                    <Column prop="desc">
                        <template #body="{ data }">
                            <span class="text-sm">{{ data.desc }}</span>
                        </template>
                    </Column>
                    <Column prop="value" class="w-50">
                        <template #body="{ data }">
                            <span class="font-semibold">{{ data.value }}</span>
                        </template>
                    </Column>
                    <Column class="w-14">
                        <template #body="{ data }">
                            <Button outlined rounded @click="onEdit(data)" size="small">
                                <icon-mdi:pencil />
                            </Button>
                        </template>
                    </Column>
                </DataTable>
            </Fieldset>
        </template>
    </Card>
</template>

<script>
export default {
    name: 'ServerConfig',
};
</script>

<script setup>
import { getServerSettings, updateServerSettings } from '~/api/gameServer';
import { myPrompt } from '~/plugins/sweetalert2';

if (!Object.groupBy) {
    Object.groupBy = function (array, callback) {
        return array.reduce((acc, item) => {
            const key = callback(item);
            if (!acc[key]) {
                acc[key] = [];
            }
            acc[key].push(item);
            return acc;
        }, {});
    };
}

const { t, locale } = useI18n();

const modelValue = ref([]);
const getData = async () => {
    const data = await getServerSettings();
    const array = [];

    Object.keys(data).forEach((key) => {
        array.push({
            name: key,
            value: data[key],
            desc: t('views.serverConfig.settings.' + key + '.desc'),
            group: t('views.serverConfig.settings.' + key + '.group'),
        });
    });

    const group = Object.groupBy(array, (item) => {
        return item.group;
    });

    array.length = 0;
    Object.keys(group).forEach((key) => {
        array.push({
            group: key,
            children: group[key],
        });
    });
    modelValue.value = array;
};
getData();

watch(locale, getData);

const getName = (str) => {
    if (!str) {
        return str;
    }
    return str.charAt(0).toUpperCase() + str.slice(1);
};

const onEdit = async (data) => {
    try {
        const name = getName(data.name);
        const value = await myPrompt({ title: name, inputValue: data.value });
        const dict = {};
        dict[name] = value;
        await updateServerSettings(dict);
        await getData();
    } catch {}
};
</script>
