<?xml version="1.0"?>
<doc>
    <assembly>
        <name>PasteApeart.Dependency</name>
    </assembly>
    <members>
        <member name="T:PasteApeart.Dependency.AutoDependencyExtensions">
            <summary>
            自动执行注入
            </summary>
        </member>
        <member name="M:PasteApeart.Dependency.AutoDependencyExtensions.AddAutoDependency(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Reflection.Assembly)">
            <summary>
            扫描注入 瞬时 作用域 单例等 ITransient ISingleton IScoped
            </summary>
            <param name="services"></param>
            <param name="assembly"></param>
            <returns></returns>
        </member>
        <member name="T:PasteApeart.Dependency.AutoDependencyInjectionExtensions">
            <summary>
            自动执行注入
            </summary>
        </member>
        <member name="M:PasteApeart.Dependency.AutoDependencyInjectionExtensions.AddAutoDependencyInjection(Microsoft.Extensions.DependencyInjection.IServiceCollection,System.Reflection.Assembly)">
            <summary>
            扫描注入 瞬时 作用域 单例等
            </summary>
            <param name="services"></param>
            <param name="assembly"></param>
            <returns></returns>
        </member>
        <member name="T:PasteApeart.Dependency.IScopedDependency">
            <summary>
            会话模式
            </summary>
        </member>
        <member name="T:PasteApeart.Dependency.IScoped">
            <summary>
            会话模式
            </summary>
        </member>
        <member name="T:PasteApeart.Dependency.ISingletonDependency">
            <summary>
            单例模式
            </summary>
        </member>
        <member name="T:PasteApeart.Dependency.ISingleton">
            <summary>
            单例模式
            </summary>
        </member>
        <member name="T:PasteApeart.Dependency.ITransientDependency">
            <summary>
            瞬时
            </summary>
        </member>
        <member name="T:PasteApeart.Dependency.ITransient">
            <summary>
            瞬时
            </summary>
        </member>
    </members>
</doc>
