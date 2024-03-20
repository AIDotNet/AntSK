import subprocess
import shlex
import os
class Start(object):

    def __init__(self,model_name_or_path):
        self.model_name_or_path=model_name_or_path

    def StartCommand(self):
        os.environ['CUDA_VISIBLE_DEVICES'] = '0'
        os.environ['API_PORT'] = '8000'
        # 构建要执行的命令
        command = (
            'python api_demo.py'
            ' --model_name_or_path E:/model/Qwen1.5-0.5B-Chat_back'
            ' --template default '
        )

        # 使用shlex.split()去安全地分割命令字符串
        command = shlex.split(command)

        # 执行命令
        subprocess.run(command, shell=True)

if __name__ == "__main__":
    star= Start('model_name_or_path')
    star.StartCommand()