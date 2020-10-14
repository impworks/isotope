import deb from 'lodash.throttle';

export function Throttle(time: number): MethodDecorator {
    return function(target: Object, propertyKey: string | symbol, descriptor: TypedPropertyDescriptor<any>): TypedPropertyDescriptor<any> {
        descriptor.value = deb(descriptor.value, time)
        return descriptor;
    }
}